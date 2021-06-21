using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleWSA
{
  public class ParameterCollection : IEnumerable<Parameter>
  {
    private readonly List<Parameter> internalList = new List<Parameter>();

    private Dictionary<string, int> lookup;
    private Dictionary<string, int> lookupIgnoreCase;

    internal ParameterCollection()
    {
      this.InvalidateHashLookups();
    }

    internal void InvalidateHashLookups()
    {
      lookup = null;
      lookupIgnoreCase = null;
    }

    public Parameter Add(Parameter parameter)
    {
      this.internalList.Add(parameter);
      return parameter;
    }

    public Parameter Add(string name)
    {
      Parameter parameter = new Parameter(name);
      return this.Add(parameter);
    }

    public Parameter Add(string name, PgsqlDbType pgsqlDbType)
    {
      Parameter parameter = new Parameter(name, pgsqlDbType);
      return this.Add(parameter);
    }

    public Parameter Add(string name, PgsqlDbType pgsqlDbType, object value)
    {
      Parameter parameter = new Parameter(name, pgsqlDbType, value);
      return this.Add(parameter);
    }
    
    public int Count
    {
      get
      {
        return this.internalList.Count;
      }
    }

    IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
    {
      return ((IEnumerable<Parameter>)internalList).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Parameter>)internalList).GetEnumerator();
    }

    public Parameter this[string name]
    {
      get
      {
        int index = IndexOf(name);

        if (index == -1)
        {
          throw new IndexOutOfRangeException("Parameter not found");
        }

        return this.internalList[index];
      }
      set
      {
        int index = IndexOf(name);

        if (index == -1)
        {
          throw new IndexOutOfRangeException("Parameter not found");
        }

        Parameter oldValue = this.internalList[index];

        if (value.Name != oldValue.Name)
        {
          InvalidateHashLookups();
        }

        this.internalList[index] = value;
      }
    }

    public int IndexOf(string parameterName)
    {
      int retIndex;
      int scanIndex;

      // Using a dictionary is much faster for 5 or more items            
      if (this.internalList.Count >= 5)
      {
        if (this.lookup == null)
        {
          this.lookup = new Dictionary<string, int>();
          for (scanIndex = 0; scanIndex < this.internalList.Count; scanIndex++)
          {
            var item = this.internalList[scanIndex];
            if (!this.lookup.ContainsKey(item.Name))
            {
              this.lookup.Add(item.Name, scanIndex);
            }
          }
        }

        // Try to access the case sensitive parameter name first
        if (this.lookup.TryGetValue(parameterName, out retIndex))
        {
          return retIndex;
        }

        // Case sensitive lookup failed, generate a case insensitive lookup
        if (this.lookupIgnoreCase == null)
        {
          this.lookupIgnoreCase = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
          for (scanIndex = 0; scanIndex < this.internalList.Count; scanIndex++)
          {
            var item = this.internalList[scanIndex];

            // Store only the first of each distinct value
            if (!this.lookupIgnoreCase.ContainsKey(item.Name))
            {
              this.lookupIgnoreCase.Add(item.Name, scanIndex);
            }
          }
        }

        // Then try to access the case insensitive parameter name
        if (this.lookupIgnoreCase.TryGetValue(parameterName, out retIndex))
        {
          return retIndex;
        }

        return -1;
      }

      retIndex = -1;

      // Scan until a case insensitive match is found, and save its index for possible return.
      // Items that don't match loosely cannot possibly match exactly.
      for (scanIndex = 0; scanIndex < this.internalList.Count; scanIndex++)
      {
        var item = this.internalList[scanIndex];

        if (string.Compare(parameterName, item.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
        {
          retIndex = scanIndex;

          break;
        }
      }

      // Then continue the scan until a case sensitive match is found, and return it.
      // If a case insensitive match was found, it will be re-checked for an exact match.
      for (; scanIndex < this.internalList.Count; scanIndex++)
      {
        var item = this.internalList[scanIndex];

        if (item.Name == parameterName)
        {
          return scanIndex;
        }
      }

      return retIndex;
    }
  }
}
