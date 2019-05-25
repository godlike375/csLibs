using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

namespace csLibs
{		
	public static class Extensions
	{
		
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source,
        IEqualityComparer<T> comparer = null)
	    {
	        return new HashSet<T>(source, comparer);
	    }
		
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
	
	public class Neuron
    {
    	public static float Sigmoid(float res)
 		{
 			if(res > 1)
 				return 1.0f;
 			if(res < 0)
 				return 0;
 			return res;
 		}
    	public delegate float active(float res);
    	//тормозящие связи = отрицательные веса
    	public float power;
    	public List<float> w = new List<float>();
    	public List<Neuron> d = new List<Neuron>();
    	public List<Tuple<Neuron, int>> a = new List<Tuple<Neuron, int>>();
    	public Neuron(int ka=1, int kd=1)
    	{
    		a = new List<Tuple<Neuron, int>>(ka);
    		w = new List<float>(kd);
    	}
    	public void Connect(Neuron to, float stW=1.0f)
    	{
    		a.Add(new Tuple<Neuron, int>(to, to.w.Count));
    		to.w.Add(stW);
    		to.d.Add(this);
    	}
    	
    	public void Disconnect(int acs)
    	{
    		a[acs].Item1.w.RemoveAt(a[acs].Item2);
    		a[acs].Item1.d.RemoveAt(a[acs].Item2);
    		a.RemoveAt(acs);
    		
    	}
    	
    	public void Push(Func<float, float> ac)
    	{
    		if(ac(power)>0)       			
    		foreach(var n in a)
    			n.Item1.power+=n.Item1.w[n.Item2]*power;
    	}
    }
	
	public class NeuralNet
	{
		public List<List<Neuron>> layers = new List<List<Neuron>>();
		public void AddLayer(int cnt=1)
		{
			layers.Add(new List<Neuron>(cnt));
			for(int i=0; i<cnt; i++)
				layers[layers.Count-1].Add(new Neuron(1, (layers.Count-2) > 0 ? layers[layers.Count-1].Count:1));
		}
		public static List<Neuron> ParallelCreate(int thCnt, int neurCnt)
		{
			int cnt = neurCnt/thCnt;
			var lists = new List<List<Neuron>>(thCnt);
			var capL = new List<int>();
			for(int i=0; i<thCnt; i++)
			{
				if(neurCnt%thCnt>0 && i==thCnt-1)
				{
					lists.Add(new List<Neuron>(cnt+neurCnt%thCnt));
					capL.Add(cnt+neurCnt%thCnt);
				}
				else
				{
					lists.Add( new List<Neuron>(cnt));
					capL.Add(cnt);
				}
			}
			var t = new Task[thCnt];
			var s = new Stack<int>();
			for(int i=0; i<thCnt; i++)
				s.Push(i);
			var acc=0;
			foreach(var ss in capL)
				acc+=ss;
			if(acc!=neurCnt)
				Console.WriteLine("lol "+acc.ToString());
			for(int i=0; i<thCnt; i++)
			{
				t[i]=Task.Factory.StartNew(()=>
        	{
        		int k = s.Pop();
        		for(int j=0; j<capL[k]; j++)
				lists[k].Add(new Neuron(1,1));
        	});	
			}
			Task.WaitAll(t);
			
			var res = new List<Neuron>(neurCnt);
			for(int i=0; i<thCnt; i++)
				res.AddRange(lists[i]);
			return res;
		}
		
		public void Iterate()
		{
			foreach(var l in layers)
				ParallelPush(l, Neuron.Sigmoid);
		}

		public static void ParallelPush(List<Neuron> ne, Func<float, float> ac)
		{
			foreach(var n in ne)
				n.Push(ac);
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
	  	int next;
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
	
	/////////////////////////////////////////////////////////
	
	
	public class Approximate
    {
    	public double[] result;
    	public int basis;
		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < basis; i++)
            {
                if (Math.Round(result[i], 3) != 0)
                    s += ((result[i] > 0) ? "+" : "") +
                        Math.Round(result[i], 3).ToString() + ((i > 0) ? "*x^" + i.ToString() : "") + " ";
            }
			return s;
		}
    	public double Predict(double x)
    	{
    		double y = 0;
            for (int i = 0; i < basis; i++)
            {
                y += result[i] * Math.Pow(x, i);
            }
            return y;
    	}
    	public Approximate(double[] x, double[] y, int ba)
    	{
    		basis = ba;
    		double[,] xy = new double[x.Length, x.Length];
    		for(int i=0; i<x.Length; i++)
    			xy[0,i] = x[i];
    		for(int i=0; i<x.Length; i++)
    			xy[1,i] = y[i];
    		var matrix = MakeSystem(xy, ba);
    		result = Gauss(matrix, basis, basis + 1);
    	}
    	
    	private double[,] MakeSystem(double[,] xyTable, int basis)
        {
            double[,] matrix = new double[basis, basis + 1];
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    double sumA = 0, sumB = 0;
                    for (int k = 0; k < xyTable.GetLength(0); k++)
                    {
                        sumA += Math.Pow(xyTable[0, k], i) * Math.Pow(xyTable[0, k], j);
                        sumB += xyTable[1, k] * Math.Pow(xyTable[0, k], i);
                    }
                    matrix[i, j] = sumA;
                    matrix[i, basis] = sumB;
                }
            }
            return matrix;
        }
    	
        private double[] Gauss(double[,] matrix, int rowCount, int colCount)
        {
            int i;
            int[] mask = new int[colCount - 1];
            for (i = 0; i < colCount - 1; i++) mask[i] = i;
            if (GaussDirectPass(ref matrix, ref mask, colCount, rowCount))
            {
                double[] answer = GaussReversePass(ref matrix, mask, colCount, rowCount);
                return answer;
            }
            else return null;
        }

        private bool GaussDirectPass(ref double[,] matrix, ref int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k, maxId, tmpInt;
            double maxVal, tempDouble;
            for (i = 0; i < rowCount; i++)
            {
                maxId = i;
                maxVal = matrix[i, i];
                for (j = i + 1; j < colCount - 1; j++)
                    if (Math.Abs(maxVal) < Math.Abs(matrix[i, j]))
                    {
                        maxVal = matrix[i, j];
                        maxId = j;
                    }
                if (maxVal == 0) return false;
                if (i != maxId)
                {
                    for (j = 0; j < rowCount; j++)
                    {
                        tempDouble = matrix[j, i];
                        matrix[j, i] = matrix[j, maxId];
                        matrix[j, maxId] = tempDouble;
                    }
                    tmpInt = mask[i];
                    mask[i] = mask[maxId];
                    mask[maxId] = tmpInt;
                }
                for (j = 0; j < colCount; j++) matrix[i, j] /= maxVal;
                for (j = i + 1; j < rowCount; j++)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            }
            return true;
        }

        private double[] GaussReversePass(ref double[,] matrix, int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k;
            for (i = rowCount - 1; i >= 0; i--)
                for (j = i - 1; j >= 0; j--)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            double[] answer = new double[rowCount];
            for (i = 0; i < rowCount; i++) answer[mask[i]] = matrix[i, colCount - 1];
            return answer;
        }    	
    	
    }
    
        public struct Data
		{
		    public List<double> x;
		    public double y;
		    public Data(List<double> xx, double yy)
		    {
		    	x = xx; y = yy;
		    }
		}
    
	public class ApproxMulti
    {

    	double delta(List<double> a, List<double> b, List<double> dx)
		{
		    var s = 0.0;
		    var i = 0;
		    for (; i < a.Count && i < b.Count && i < dx.Count; i++) if (dx[i] > 0.0) s += (a[i] - b[i]) * (a[i] - b[i]) / dx[i];
		    for (; i < a.Count && i < dx.Count; i++) if (dx[i] > 0.0) s += a[i] * a[i] / dx[i];
		    for (; i < b.Count && i < dx.Count; i++) if (dx[i] > 0.0) s += b[i] * b[i] / dx[i];
		    return s;
		}
    	double scalar(List<double> a, List<double> b, List<double> dx)
		{
		    var s = 0.0;
		    var i = 0;
		    for (; i < a.Count && i < b.Count && i < dx.Count; i++) if (dx[i] > 0.0) s += (a[i] * b[i]) / dx[i];
		    return s;
		}
    	
    	void ListResize<T>(List<T> ar, int sz)
    	{
    		int count = ar.Count;
    		if(sz != count)
    			ar.RemoveRange(sz+1, count-sz);
    	}
    	
    	public double predict(List<double> x,
               List<Data> previous_results,
               List<double> dx,
               int p)
		{
    		var neighbors = new List<KeyValuePair<Data, double>>();
		    foreach (var it in previous_results)
		    {
		        List<double> x2 = it.x;
		        var d = delta(x, x2, dx);
		        var pair = new KeyValuePair<Data, double>(it, d);
		        neighbors.Add(pair);
		    }
		    neighbors.Sort( (i, j) => i.Value.CompareTo(j.Value));
		    ListResize(neighbors, p<neighbors.Count? p : neighbors.Count);
		    var y = 0.0;
		    foreach(var iti in neighbors)
		    {
		        var s = iti.Key.y;
		        List<double> xi = iti.Key.x;
		        foreach(var itj in neighbors)
		        {
		        	if (iti.Equals(itj)) continue;
		            List<double> xj = itj.Key.x;
		            var xxj = new List<double>();
		            var xixj = new List<double>();
		            for (var i = 0; i < x.Count && i < xj.Count; i++) xxj.Add(x[i] - xj[i]);
		            for (var i = 0; i < xi.Count && i < xj.Count; i++) xixj.Add(xi[i] - xj[i]);
		            s *= scalar(xxj, xixj, dx) / scalar(xixj, xixj, dx);
		        }
		        y += s;
		    }
		    return y;
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
