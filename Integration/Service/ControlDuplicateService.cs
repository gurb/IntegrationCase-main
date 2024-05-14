using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
	// singleton service
	public class ControlDuplicateService
	{
		public ConcurrentDictionary<string, bool> Register { get; set; }

        public ControlDuplicateService()
        {
            Register = new ConcurrentDictionary<string, bool>();
        }

        public void AddContent(string content)
        {
            if (IsExistContent(content))
                return;

            Register.TryAdd(content, true);
        }

        public bool IsExistContent(string content)
        {
            return Register.ContainsKey(content);
        }

		public void RemoveContent(string content)
		{
			if (!IsExistContent(content))
				return;
            bool value = true;
			Register.TryRemove(content, out value);
		}
	}
}
