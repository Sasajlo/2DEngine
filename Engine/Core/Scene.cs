using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace Engine
{
    public class Scene
    {
        public string name { get; set; }
        private List<GameObject> _gameObjects = new List<GameObject>();
        
        public Scene(string name = "Scene")
        {
            this.name = name;
        }
        
        public GameObject CreateGameObject(string name = "GameObject")
        {
            var gameObject = new GameObject(name);
            gameObject.scene = this;
            _gameObjects.Add(gameObject);
            return gameObject;
        }
        
        public void AddGameObject(GameObject gameObject)
        {
            if (!_gameObjects.Contains(gameObject))
            {
                gameObject.scene = this;
                _gameObjects.Add(gameObject);
            }
        }
        
        public void RemoveGameObject(GameObject gameObject)
        {
            if (_gameObjects.Remove(gameObject))
            {
                gameObject.scene = null;
            }
        }
        
        public T FindObjectOfType<T>() where T : Component
        {
            foreach (var gameObject in _gameObjects.ToList())
            {
                var component = gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }
        
        public T[] FindObjectsOfType<T>() where T : Component
        {
            var results = new List<T>();
            foreach (var gameObject in _gameObjects.ToList())
            {
                var component = gameObject.GetComponent<T>();
                if (component != null)
                {
                    results.Add(component);
                }
            }
            return results.ToArray();
        }
        
        public GameObject FindGameObject(string name)
        {
            return _gameObjects.FirstOrDefault(go => go.name == name);
        }
        
        public GameObject[] GetAllGameObjects()
        {
            return _gameObjects.ToArray();
        }
        
        public void AwakeAll()
        {
            foreach (var gameObject in _gameObjects.ToList())
            {
                gameObject.AwakeAll();
            }
        }
        
        public void Start()
        {
            foreach (var gameObject in _gameObjects.ToList())
            {
                gameObject.Start();
            }
        }
        
        public void Update()
        {
            foreach (var gameObject in _gameObjects.ToList())
            {
                gameObject.Update();
            }
        }
        
        public void Render()
        {
            // Get all sprite renderers and sort by sorting order
            var spriteRenderers = FindObjectsOfType<SpriteRenderer>()
                .OrderBy(sr => sr.sortingOrder)
                .ToArray();
            
            // Get all world managers for tile rendering
            var worldManagers = FindObjectsOfType<WorldManager>();
            
            // Render all sprites
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.Render();
            }
            
            // Render tiles from all world managers
            foreach (var worldManager in worldManagers)
            {
                worldManager.Render();
            }
        }
        
        public void Clear()
        {
            foreach (var gameObject in _gameObjects.ToArray())
            {
                gameObject.Destroy();
            }
            _gameObjects.Clear();
        }
    }
} 