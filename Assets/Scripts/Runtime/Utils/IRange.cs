using UnityEngine;

namespace Starblast.Utils
{
    public interface IRange<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }
    }
    
    [System.Serializable]
    public class Range<T> : IRange<T>
    {
        [SerializeField] protected T _min = default;
        [SerializeField] protected T _max = default;
        
        public T Min
        {
            get => _min; 
            set => _min = value;
        }

        public T Max { 
            get => _max;
            set => _max = value;
        }

        public Range()
        {
            _min = default;
            _max = default;
        }
        
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }
}