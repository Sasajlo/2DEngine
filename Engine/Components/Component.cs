using System;

namespace Engine.Components
{
    public abstract class Component
    {
        public GameObject gameObject { get; internal set; }
        public bool enabled { get; set; } = true;
        
        protected Component()
        {
        }
        
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }
    }
} 