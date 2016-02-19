using System;

namespace Entities.Model.Components
{
	public class EntityName : Component
	{
		private string nameString;

		public virtual string NameString
		{
			get { return nameString; }
		}

		public EntityName(string name)
		{
			nameString = name;
		}
	}
}
