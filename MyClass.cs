using System;
using System.Collections.Generic;
using System.Collections;

namespace extensions
{		
	public static class Extensions
	{
		
	    public static Range<T> Range<T>(this T[] source, int start, int len)
	    {
	    	return new Range<T>(source, start, len);
	    }

	    public static StringRange Range(this string source, int start, int len)
	    {
	    	return new StringRange(source, start, len);
	    }	    
		
	    public static T[] Slice<T>(this T[] source, int start, int len)
	    {
	        T[] res = new T[len];
	        for (int i = 0; i < len; i++)
	        {
	            res[i] = source[i + start];
	        }
	        return res;
	    }
	   	public static string Slice(this string source, int start, int len)
	    {
	   		char[] res = new char[len];
	        for (int i = 0; i < len; i++)
	        {
	            res[i] = source[i + start];
	        }
	        return new string(res);
	    }
	}
	
	public class pair<O,T>
	{
		public O o;
		public T t;
		public pair(O oo, T tt)
		{
			this.o = oo;
			this.t = tt;
		}
	}
	
	public class CircularBuffer<T>
	{
		public T[] buffer;
	  	public int next;
	  	int count;
	  	public int Count{get{return count;}}
	
		public T this[int i]
		{
			
			get 
			{
				int cnt = count > i ? i : count-1;
				int ini = (next-count+cnt);
				ini = ini<0? count-(ini*-1) : ini;
				return buffer[ini];
			}
		}
	  	
		public CircularBuffer(int length)
		{
			buffer = new T[length];
			next = 0;
			count = 0;
		}
		
		public void Add(T o)
		{
			buffer[next] = default(T);
			buffer[next] = o;
			next = (next+1) % buffer.Length;
			count = count < buffer.Length ? count+1: buffer.Length;
		}
	}
	
	////////////////////////////////////////////////////////////
		public struct Range<T>:IEnumerable<T>
	{
		T[] arr;
		int s;
		int l;
		public Range(T[] array, int start, int length)
		{
			if(start>=0 && start+length > array.Length)
				throw(new ArgumentOutOfRangeException());
			arr = array;
			s = start;
			l = length;
		}
		public IEnumerator<T> GetEnumerator()
		{
			return new RangeEnumerator<T>(arr, s, l);
		}
		
		public T[] getArray()
		{
			return arr.Slice(s, l);
		}
		
		public T this[int index]
		{
			get
	        {
				if(index>=0 && index<l)
				{
					return arr[s+index];
				}
				throw(new ArgumentOutOfRangeException());
	        }

	        set
	        {
	            if(index>=0 && index<l)
				{
					arr[s+index] = value;
				}
				throw(new ArgumentOutOfRangeException());
	        }
		}
		
		public Range<T> getSubRange(int start, int length)
		{
			if(start>=0 && s+start+length > arr.Length)
				throw(new ArgumentOutOfRangeException());
			return new Range<T>(arr, s+start, length);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
	    {
	        return this.GetEnumerator();
	    }
	    
	}
	public struct RangeEnumerator<T>:IEnumerator<T>
	{
		T[] arr;
		int s;
		int l;
		int cur;
		public RangeEnumerator(T[] array, int start, int length)
		{
			arr = array;
			s = start;
			l = length;
			cur = -1;
		}
		public T Current
		{
			get
			{
				return arr[s+cur];
			}
		}
		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}
		public bool MoveNext()
		{
			return ++cur<l;
		}
		public void Reset()
		{
			cur=-1;
		}
		 public void Dispose()
    	{
		 	arr = null;
		}
	}
	
	public struct StringRange:IEnumerable<char>
	{
		string arr;
		int s;
		int l;
		public StringRange(string array, int start, int length)
		{
			if(start>=0 && start+length > array.Length)
				throw(new ArgumentOutOfRangeException());
			arr = array;
			s = start;
			l = length;
			
		}
		public IEnumerator<char> GetEnumerator()
		{
			return new StringRangeEnumerator(arr, s, l);
		}
		
		public string getString()
		{
			return arr.Slice(s, l);
		}
		
		public char this[int index]
		{
			get
	        {
				if(index>=0 && index<l)
				{
					return arr[s+index];
				}
				throw(new ArgumentOutOfRangeException());
	        }
		}
		
		public StringRange getSubRange(int start, int length)
		{
			if(start>=0 && s+start+length > arr.Length)
				throw(new ArgumentOutOfRangeException());
			return new StringRange(arr, s+start, length);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
	    {
	        return this.GetEnumerator();
	    }
	    
	}
	public struct StringRangeEnumerator:IEnumerator<char>
	{
		string arr;
		int s;
		int l;
		int cur;
		public StringRangeEnumerator(string array, int start, int length)
		{
			arr = array;
			s = start;
			l = length;
			cur = -1;
		}
		public char Current
		{
			get
			{
				return arr[s+cur];
			}
		}
		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}
		public bool MoveNext()
		{
			return ++cur<l;
		}
		public void Reset()
		{
			cur=-1;
		}
		 public void Dispose()
    	{
		 	arr = null;
		}
	}
}
