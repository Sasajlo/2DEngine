using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace Engine
{
    public class GameObject
    {
        public string name { get; set; }
        public bool active { get; set; } = true;
        public Scene scene { get; internal set; }
        
        private List<Component> _components = new List<Component>();
        private Transform _transform;
        private bool _hasAwakened = false;
        
        // Unity-style direct access to transform
        public Transform transform => _transform;
        
        public GameObject(string name = "GameObject")
        {
            this.name = name;
            
            // Every GameObject must have a Transform component (Unity-style)
            _transform = new Transform();
            _transform.gameObject = this;
            _components.Add(_transform);
            // Note: Awake() will be called later in AwakeAll()
        }
        
        public T AddComponent<T>() where T : Component, new()
        {
            // Transform component is automatically added and cannot be added again
            if (typeof(T) == typeof(Transform))
            {
                return _transform as T;
            }
            
            // Check if component already exists
            var existing = GetComponent<T>();
            if (existing != null)
            {
                return existing;
            }
            
            var component = new T();
            component.gameObject = this;
            _components.Add(component);
            
            // Note: Awake() will be called later in AwakeAll()
            
            return component;
        }
        
        public T GetComponent<T>() where T : Component
        {
            return _components.OfType<T>().FirstOrDefault();
        }
        
        public T[] GetComponents<T>() where T : Component
        {
            return _components.OfType<T>().ToArray();
        }
        
        public Component[] GetAllComponents()
        {
            return _components.ToArray();
        }
        
        public bool RemoveComponent<T>() where T : Component
        {
            // Transform component cannot be removed (Unity-style)
            if (typeof(T) == typeof(Transform))
            {
                return false;
            }
            
            var component = GetComponent<T>();
            if (component != null)
            {
                component.OnDestroy();
                return _components.Remove(component);
            }
            return false;
        }
        
        public void AwakeAll()
        {
            if (!active || _hasAwakened) return;
            
            foreach (var component in _components)
            {
                if (component.enabled)
                {
                    component.Awake();
                }
            }
            
            _hasAwakened = true;
        }
        
        public void Start()
        {
            if (!active) return;
            
            foreach (var component in _components)
            {
                if (component.enabled)
                {
                    component.Start();
                }
            }
        }
        
        public void Update()
        {
            if (!active) return;
            
            foreach (var component in _components)
            {
                if (component.enabled)
                {
                    component.Update();
                }
            }
        }
        
        public void Destroy()
        {
            foreach (var component in _components)
            {
                component.OnDestroy();
            }
            _components.Clear();
            
            scene?.RemoveGameObject(this);
        }
    }
} 