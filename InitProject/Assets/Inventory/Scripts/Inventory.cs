using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Inventories
{
    public sealed class Inventory : IEnumerable<Item>
    {
        public event Action<Item, Vector2Int> OnAdded;
        public event Action<Item, Vector2Int> OnRemoved;
        public event Action<Item, Vector2Int> OnMoved;
        public event Action OnCleared;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Count => _items.Count;

        private readonly Dictionary<Vector2Int, Item> _items = new Dictionary<Vector2Int, Item>();

        public Inventory(int width, int height)
        {
            if(width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException();

            Width = width;
            Height = height;
        }

        public Inventory(int width, int height, params KeyValuePair<Item, Vector2Int>[] items)
            : this(width, height)
        {
            if(items is null) throw new ArgumentNullException();
            foreach(var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(int width, int height, params Item[] items)
            : this(width, height)
        {
            if(items is null) throw new ArgumentNullException();
            foreach(var item in items)
                AddItem(item);
        }

        public Inventory(int width, int height, IEnumerable<KeyValuePair<Item, Vector2Int>> items)
            : this(width, height)
        {
            if(items is null) throw new ArgumentNullException();
            foreach(var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(int width, int height, IEnumerable<Item> items)
            : this(width, height)
        {
            if(items is null) throw new ArgumentNullException();
            foreach(var item in items)
                AddItem(item);
        }

        public bool CanAddItem(Item item, Vector2Int position)
        { 
            if(item == null)
                return false;

            if(item.Size.x <= 0 || item.Size.y <= 0)
                throw new ArgumentException();

            return !(_items.ContainsValue(item) || !IsWithinBounds(position, item.Size) ||  IntersectsWithExistingItems(item, position));
        }


        public bool CanAddItem(Item item, int posX, int posY)
            => CanAddItem(item, new Vector2Int(posX, posY));

        public bool AddItem(Item item, Vector2Int position)
            => AddItem(item, position.x, position.y);

        public bool AddItem(Item item, int x, int y)
        {
            if(item == null || _items.ContainsValue(item) || !CanAddItem(item, x, y))
                return false;  

            var newPosition = new Vector2Int(x, y);
            _items.Add(newPosition, item);
            OnAdded?.Invoke(item, newPosition);

            return true;
        }

        public bool CanAddItem(Item item)
        {
            if(item == null)
               return false;

            if(item.Size.x <= 0 || item.Size.y <= 0)
                throw new ArgumentException();  

            if(_items.ContainsValue(item))
                return false;

            return FindFreePosition(item.Size, out _);
        }

        public bool AddItem(Item item)
        {
            if(item == null)
                return false;

            if(!FindFreePosition(item.Size, out Vector2Int position))
                return false;

            return AddItem(item, position);
        }

        public bool FindFreePosition(Vector2Int size, out Vector2Int freePosition)
        {
            if(size.x <= 0 || size.y <= 0)
                throw new ArgumentOutOfRangeException();

            for(int y = 0; y <= Height - size.y; y++)
            {
                for(int x = 0; x <= Width - size.x; x++)
                {
                    var position = new Vector2Int(x, y);
                    if(CanAddItem(new Item(size), position))
                    {
                        freePosition = position;
                        return true;
                    }
                }
            }

            freePosition = Vector2Int.zero;
            return false;
        }


        public bool Contains(Item item)
        {
            if(item == null)
                return false;

            return _items.ContainsValue(item);
        }

        public bool IsOccupied(Vector2Int position)
            => IsOccupied(position.x, position.y);

        public bool IsOccupied(int x, int y)
        {
            return _items.Any(kvp =>
            {
                var itemPos = kvp.Key;
                var itemSize = kvp.Value.Size;
                return x >= itemPos.x && x < itemPos.x + itemSize.x &&
                       y >= itemPos.y && y < itemPos.y + itemSize.y;
            });
        }

        public bool IsFree(Vector2Int position)
            => !IsOccupied(position.x, position.y);

        public bool IsFree(int x, int y)
            => !IsOccupied(x, y);

        public bool RemoveItem(Item item)
        {
            if(item == null)
                return false;

            if(_items.ContainsValue(item))
            {
                var position = _items.First(kvp => kvp.Value.Equals(item)).Key;
                _items.Remove(position);
                OnRemoved?.Invoke(item, position);
                return true;
            }

            return false;
        }

        public bool RemoveItem(Item item, out Vector2Int position)
        {
            if(item == null || !_items.ContainsValue(item))
            {
                position = Vector2Int.zero;
                return false;
            }

            position = _items.First(kvp => kvp.Value.Equals(item)).Key;
            _items.Remove(position);
            OnRemoved?.Invoke(item, position);
            return true;
        }

        public Item GetItem(Vector2Int position)
            => GetItem(position.x, position.y);

        public Item GetItem(int x, int y)
        { 
            if(x < 0 || y < 0 || x >= Width || y >= Height)
                throw new IndexOutOfRangeException();

            foreach(var kvp in _items)
            {
                var itemPos = kvp.Key;
                var itemSize = kvp.Value.Size;
                 
                if(x >= itemPos.x && x < itemPos.x + itemSize.x &&
                    y >= itemPos.y && y < itemPos.y + itemSize.y)
                {
                    return kvp.Value;
                }
            }
             
            throw new NullReferenceException();
        }


        public bool TryGetItem(Vector2Int position, out Item item)
            => TryGetItem(position.x, position.y, out item);

        public bool TryGetItem(int x, int y, out Item item)
        {
            item = null;
             
            if(x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            foreach(var kvp in _items)
            {
                var itemPos = kvp.Key;
                var itemSize = kvp.Value.Size;
                 
                if(x >= itemPos.x && x < itemPos.x + itemSize.x &&
                    y >= itemPos.y && y < itemPos.y + itemSize.y)
                {
                    item = kvp.Value;
                    return true;
                }
            } 
            return false;
        }


        public Vector2Int[] GetPositions(Item item)
        {
            if(item == null)
                throw new NullReferenceException();

            if(!_items.ContainsValue(item))
                throw new KeyNotFoundException();

            var result = new List<Vector2Int>();
            var position = _items.First(kvp => kvp.Value.Equals(item)).Key;

            for(int x = 0; x < item.Size.x; x++)
            {
                for(int y = 0; y < item.Size.y; y++)
                {
                    result.Add(new Vector2Int(position.x + x, position.y + y));
                }
            }

            return result.ToArray();
        }

        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        { 
            if(item == null)
            {
                positions = null;
                return false;
            }
             
            if(!_items.ContainsValue(item))
            {
                positions = null;
                return false;
            } 
            positions = GetPositions(item);
            return true;
        }


        public void Clear()
        {
            if(_items.Count > 0)
            {
                _items.Clear();
                OnCleared?.Invoke();
            }
        }

        public int GetItemCount(string name)
        {
            return _items.Values.Count(item => item.Name == name);
        }

        public bool MoveItem(Item item, Vector2Int newPosition)
        {
            if(item == null)
                throw new ArgumentNullException();

            if(!_items.ContainsValue(item) || !IsWithinBounds(newPosition, item.Size) || IntersectsWithOtherItems(item, newPosition))
                return false; 

            var oldPosition = _items.First(kvp => kvp.Value.Equals(item)).Key;
            _items.Remove(oldPosition);

            _items[newPosition] = item;

            OnMoved?.Invoke(item, newPosition);
            return true;
        }



        public void ReorganizeSpace()
        { 
            var itemsToReorganize = _items.ToList();
             
            Clear();
             
            itemsToReorganize.Sort((a, b) =>
            {
                int areaA = a.Value.Size.x * a.Value.Size.y;
                int areaB = b.Value.Size.x * b.Value.Size.y;

                int comparison = areaB.CompareTo(areaA);
                return comparison != 0 ? comparison : string.Compare(a.Value.Name, b.Value.Name, StringComparison.Ordinal);
            });
             
            foreach(var kvp in itemsToReorganize)
            {
                var item = kvp.Value;
                 
                if(!FindFreePosition(item.Size, out var freePosition))
                    throw new InvalidOperationException();
                 
                AddItem(item, freePosition);
            }
        }



        public void CopyTo(Item[,] matrix)
        {
            if(matrix.GetLength(0) != Width || matrix.GetLength(1) != Height)
                throw new ArgumentException();

            foreach(var kvp in _items)
            {
                var position = kvp.Key;
                var item = kvp.Value;

                for(int x = 0; x < item.Size.x; x++)
                {
                    for(int y = 0; y < item.Size.y; y++)
                    {
                        matrix[position.x + x, position.y + y] = item;
                    }
                }
            }
        }

        public IEnumerator<Item> GetEnumerator()
            => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private bool IsWithinBounds(Vector2Int position, Vector2Int size)
        {
            return position.x >= 0 && position.y >= 0 &&
                   position.x + size.x <= Width &&
                   position.y + size.y <= Height;
        }

        private bool IntersectsWithExistingItems(Item item, Vector2Int position)
        {
            var newRect = new RectInt(position, item.Size);

            foreach(var kvp in _items)
            {
                var existingRect = new RectInt(kvp.Key, kvp.Value.Size);
                if(newRect.Overlaps(existingRect))
                    return true;
            }

            return false;
        }
        private bool IntersectsWithOtherItems(Item item, Vector2Int newPosition)
        {
            var newRect = new RectInt(newPosition, item.Size);

            foreach(var kvp in _items)
            {
                if(kvp.Value.Equals(item))
                    continue;

                var existingRect = new RectInt(kvp.Key, kvp.Value.Size);
                if(newRect.Overlaps(existingRect))
                    return true;
            }

            return false;
        }

    }
}
