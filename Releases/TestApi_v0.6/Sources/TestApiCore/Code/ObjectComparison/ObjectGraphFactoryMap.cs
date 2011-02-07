// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents a map of {<see cref="Type"/> -> <see cref="ObjectGraphFactory"/>}.
    /// </summary>
    /// <remarks>
    /// <see cref="ObjectGraphFactoryMap"/> works only with generic definitions of generic types.
    /// That is some operation on the map is invoked for the type <c>MyType&lt;int&gt;</c>
    /// that operation will be actually performed for <c>MyType&lt;T&gt;</c>.
    /// See <see cref="System.Type.IsGenericTypeDefinition"/> for more details.
    /// </remarks>
    public sealed class ObjectGraphFactoryMap : IDictionary<Type, ObjectGraphFactory>
    {
        /// <summary>
        /// Constructs a new instaince of <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        /// <param name="exactTypeMatch">
        /// The value indicating whether exact type match should be performed. 
        /// If it equals <see lang="true"/> the type match is exact. That is, type B matches only
        /// type B and does not match any other type.
        /// If it equals <see lang="false"/>, types are matched based on their type compatibility. 
        /// That is, type B matches not only itself, but also any other type compatible with it. 
        /// E.g., type D, derived from B. That effectively allows factories to be registered 
        /// not only for types, but for interfaces as well. In case there is no exact match for a
        /// type, a factory for the 'closest' compatible type is chosen. That type is determined by 
        /// the following algorithm:
        /// <list type="number">
        /// <item>
        ///     <description>
        ///     The map is searched for all base type of the type in question (including <see cref="Object"/>) in
        ///     the order of the inheritance chain. If the map contains an entry for a certain base type, the
        ///     corresponding <see cref="ObjectGraphFactory"/> is returned, if not the next step is taken.
        ///     </description>
        /// </item>
        /// <item>
        ///     <description>
        ///     The map is searched for types of interfaces implemented by the type in question.
        ///     If the map contains an entry for a certain interface the corresponding 
        ///     <see cref="ObjectGraphFactory"/> is returned. If no entries are found the 
        ///     <see cref="KeyNotFoundException"/> is thrown. If more than one entry is found
        ///     the <see cref="ArgumentException"/> is thrown.
        ///     </description>
        /// </item>
        /// </list>
        /// </param>
        public ObjectGraphFactoryMap(bool exactTypeMatch)
        {
            this.exactTypeMatch = exactTypeMatch;
            this.map = new Dictionary<Type, ObjectGraphFactory>();
        }

        #region IDictionary<Type,ObjectGraphFactory> Members

        /// <summary>
        /// Registers the specified object graph factory for the given type.
        /// </summary>
        /// <param name="key">The type to register the factory for.</param>
        /// <param name="value">The object graph factory to be registered.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// an element with the same key already exists in the <see cref="ObjectGraphFactoryMap"/>
        /// </exception>
        /// <remarks>
        /// If <paramref name="key"/> is not a generic type definition, an entry for its corresponding
        /// generic type definition will be added (<see cref="System.Type.IsGenericTypeDefinition"/>).
        /// </remarks>
        public void Add(Type key, ObjectGraphFactory value)
        {
            var keyType = GetGenericTypeDefinition(key);
            map.Add(keyType, value);
        }

        /// <summary>
        /// Determines whether the <see cref="ObjectGraphFactoryMap"/> contains an 
        /// <see cref="ObjectGraphFactory"/> for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="ObjectGraphFactoryMap"/>.</param>
        /// <returns>
        /// <see lang="true"/> if the <see cref="ObjectGraphFactoryMap"/> contains 
        /// an <see cref="ObjectGraphFactory"/> for the specified key; otherwise, 
        /// <see lang="false"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// Note that search is always performed for generic type definitions of types. 
        /// That is if the type specified by <paramref name="key"/> is <c>MyType&lt;int&gt;</c>
        /// the actual search will be performed for <c>MyType&lt;T&gt;</c>.
        /// <para/>Note that if this method returns <see lang="true"/> for a given type,
        /// then <see cref="TryGetValue(Type, out ObjectGraphFactory)"/> or <see cref="this[Type]"/> 
        /// can thrown an <see cref="ArgumentException"/> exception for the same type if a non-exact
        /// type match is on and there are ambiguous entries in the map. 
        /// See <see cref="TryGetValue(Type, out ObjectGraphFactory)"/> for more details.
        /// </remarks>
        public bool ContainsKey(Type key)
        {
            ObjectGraphFactory value;
            return TryGetValue(key, out value, false);
        }

        /// <summary>
        /// Gets a collection containing the types with registered <see cref="ObjectGraphFactory"/>.
        /// </summary>
        /// <returns>
        /// A collection containing the types with registered <see cref="ObjectGraphFactory"/>.
        /// </returns>
        public ICollection<Type> Keys
        {
            get { return map.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <see lang="true"/> if the element is successfully removed; 
        /// otherwise, <see lang="false"/>.  
        /// This method also returns false if <paramref name="key"/> was not found in the original 
        /// <see cref="ObjectGraphFactoryMap"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is <see lang="null"/>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="key"/> is not a generic type definition, an entry for its corresponding
        /// generic type definition will be removed (<see cref="System.Type.IsGenericTypeDefinition"/>).
        /// </remarks>
        public bool Remove(Type key)
        {
            var keyType = GetGenericTypeDefinition(key);
            return map.Remove(keyType);
        }

        /// <summary>
        /// Gets the object graph factory associated with the given type.
        /// </summary>
        /// <param name="key">The type to get an object graph factory for.</param>
        /// <param name="value">The object graph factory for the specified type.</param>
        /// <returns>
        /// <see lang="true"/> if the map contains an object graph factory for the specified 
        /// type or a compatible type, depending on what kind of search was performed;
        /// otherwise, <see lang="false"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is <see lang="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if the map does not contain any entries for base types of <paramref name="key"/>, but does
        /// contain entries for at least two interfaces implemented by <paramref name="key"/> and 
        /// those entries contain <see cref="ObjectGraphFactory"/> of different types. Since
        /// no order applies to interface types, there is an ambiguity about which 
        /// <see cref="ObjectGraphFactory"/> should be returned.
        /// </exception>
        /// <remarks>
        /// If <paramref name="key"/> is not a generic type definition, an entry for its corresponding
        /// generic type definition will be removed (<see cref="System.Type.IsGenericTypeDefinition"/>).
        /// </remarks>
        public bool TryGetValue(Type key, out ObjectGraphFactory value)
        {
            return TryGetValue(key, out value, true);
        }

        /// <summary>
        /// Gets a collection of registered object graph factories.
        /// </summary>
        /// <returns>
        /// A collection of registered object graph factories.
        ///   </returns>
        public ICollection<ObjectGraphFactory> Values
        {
            get { return map.Values; }
        }

        /// <summary>
        /// Gets or sets the factory for the specified type.
        /// </summary>
        /// <returns>
        /// The <see cref="ObjectGraphFactory"/> for the specified key.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="key"/> is not found.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if the map does not contain any entries for base types of <paramref name="key"/>, but does
        /// contain entries for at least two interfaces implemented by <paramref name="key"/> and 
        /// those entries contain <see cref="ObjectGraphFactory"/> of different types. Since
        /// no order applies to interface types, there is an ambiguity about which 
        /// <see cref="ObjectGraphFactory"/> should be returned.
        /// </exception>
        public ObjectGraphFactory this[Type key]
        {
            get
            {
                ObjectGraphFactory value;
                if (TryGetValue(key, out value, true))
                {
                    Debug.Assert(value != null);
                    return value;
                }

                throw new KeyNotFoundException("ObjectGraphFactory for a specified type is not found.");
            }
            set
            {
                map[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<Type,ObjectGraphFactory>> Members

        /// <summary>
        /// Adds an item to the <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="ObjectGraphFactoryMap"/>.
        /// </param>
        public void Add(KeyValuePair<Type, ObjectGraphFactory> item)
        {
            map.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        public void Clear()
        {
            map.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ObjectGraphFactoryMap"/> contains a specific value.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="ObjectGraphFactoryMap"/>.
        /// </param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ObjectGraphFactoryMap"/>; 
        /// otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<Type, ObjectGraphFactory> item)
        {
            return map.Contains(item);
        }

        /// <summary>
        /// Copies <see cref="ObjectGraphFactoryMap"/> entries to the specified array.
        /// </summary>
        /// <param name="array">The array to entries to.</param>
        /// <param name="arrayIndex">Strating index of the array.</param>
        public void CopyTo(KeyValuePair<Type, ObjectGraphFactory>[] array, int arrayIndex)
        {
            map.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ObjectGraphFactoryMap"/>.
        ///   </returns>
        public int Count
        {
            get { return map.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ObjectGraphFactoryMap"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ObjectGraphFactoryMap"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return map.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ObjectGraphFactoryMap"/>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="ObjectGraphFactoryMap"/>.
        /// </param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the 
        /// <see cref="ObjectGraphFactoryMap"/>; otherwise, false. 
        /// This method also returns false if <paramref name="item"/> is not found in the original 
        /// <see cref="ObjectGraphFactoryMap"/>.
        /// </returns>
        public bool Remove(KeyValuePair<Type, ObjectGraphFactory> item)
        {
            return map.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<Type,ObjectGraphFactory>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator{KeyValuePair{Type,ObjectGraphFactory}}"/> 
        /// that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<Type, ObjectGraphFactory>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to 
        /// iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }

        #endregion

        #region Private Helpers

        private static Type GetGenericTypeDefinition(Type key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return key.IsGenericType 
                ? key.GetGenericTypeDefinition()
                : key;
        }

        /// <summary>
        /// Tries to get an <see cref="ObjectGraphFactory"/> for the specified type and optionally
        /// throws an exception in case of ambiguity.
        /// </summary>
        private bool TryGetValue(Type key, out ObjectGraphFactory value, bool throwOnMultiple)
        {
            var keyType = GetGenericTypeDefinition(key);

            if (exactTypeMatch) return map.TryGetValue(keyType, out value);

            // Search for base type matches first
            var baseType = keyType;
            do
            {
                baseType = GetGenericTypeDefinition(baseType);
                if (map.TryGetValue(baseType, out value)) return true;
                baseType = baseType.BaseType;
            } while (baseType != null);

            // Search for interface matches
            ObjectGraphFactory found = null;
            foreach (var i in keyType.GetInterfaces())
            {
                if (map.TryGetValue(GetGenericTypeDefinition(i), out value))
                {
                    if (!throwOnMultiple) return true;

                    if (found != null && found.GetType() != value.GetType())
                    {
                        throw new ArgumentException(
                            "Map contains entries for multiple interfaces implemented by the given type.",
                            "key");
                    }

                    found = value;
                }
            }

            value = found;
            return found != null;
        }

        #endregion

        #region Private Data

        private IDictionary<Type, ObjectGraphFactory> map;
        private bool exactTypeMatch;

        #endregion
    }
}
