using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

namespace Core.User
{
    public class DataManager
    {
    
        private Dictionary<string, string> dataStorage = new();

        private string jsonPath;
    
        public bool Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                dataStorage = JsonUtility.FromJson<SerializableDictionary>(json).ToDictionary();
                return true;
            }
            jsonPath = filePath;
            return false;
        }

        public void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        public void Reset<T>() where T : IDefaultable<T>, new()
        {
            string key = typeof(T).ToString();
            T instance = new T();
            string json = JsonUtility.ToJson(instance);
            dataStorage[key] = json;
        }
    
        public void Save()
        {
            var serializableDictionary = new SerializableDictionary(dataStorage);
            string json = JsonUtility.ToJson(serializableDictionary);
            File.WriteAllText(jsonPath, json);
        }
    
        public T Get<T>() where T : IDefaultable<T>, new()
        {
            string key = typeof(T).ToString();
            if (dataStorage.TryGetValue(key, out string value))
            {
                return JsonUtility.FromJson<T>(value);
            }

            if (typeof(IDefaultable<T>).IsAssignableFrom(typeof(T)))
            {
                return ((IDefaultable<T>)Activator.CreateInstance(typeof(T))).DefaultInstance();
            }
            return new T();
        }
    
        public void Set<T>(T data) where T : IDefaultable<T>, new()
        {
            string key = typeof(T).ToString();
            string json = JsonUtility.ToJson(data);
            dataStorage[key] = json;
        }
    
        public TValue GetSpecificField<T, TValue>(Expression<Func<T, object>> selector) where T : IDefaultable<T>, new()
        {
            string fieldName = ReflectionHelper.GetFieldName(selector);

            T data = Get<T>();
            var type = typeof(T);
            var field = type.GetField(fieldName);
            if (field != null)
            {
                var keyValue = (KeyValue<string, TValue>)field.GetValue(data);
                return keyValue.value;
            }
            throw new Exception($"The type {type.Name} does not contain a field named {fieldName}.");
        }
        public static class ReflectionHelper
        {
            public static string GetFieldName<T>(Expression<Func<T, object>> selector)
            {
                if (selector.Body is MemberExpression member)
                {
                    return member.Member.Name;
                }
                if (selector.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
                {
                    return unaryMember.Member.Name;
                }

                throw new ArgumentException("Not a property or field", nameof(selector));
            }
        }
    }
}
