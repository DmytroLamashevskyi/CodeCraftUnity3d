using UnityEngine;

namespace Inventories
{
    ///Don't modify 
    public sealed class Item
    {
        private static int _iD_GEN;

        public string Name => this._name;
        public Vector2Int Size => this._size;

        private readonly Vector2Int _size;
        private readonly string _name;
        private readonly int _id;

        public Item(string name, Vector2Int size) : this()
        {
            this._name = name;
            this._size = size;
        }

        public Item(string name, int width, int height) : this()
        {
            this._name = name;
            this._size = new Vector2Int(width, height);
        }

        public Item(Vector2Int size) : this()
        {
            this._name = string.Empty;
            this._size = size;
        }

        public Item(int width, int height) : this()
        {
            this._name = string.Empty;
            this._size = new Vector2Int(width, height);
        }

        private Item()
        {
            this._id = _iD_GEN++;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public bool Equals(Item other)
        {
            return this._id == other._id;
        }

        public override int GetHashCode()
        {
            return this._id;
        }

        public override string ToString()
        {
            return $"{this._name}";
        }
    }
}