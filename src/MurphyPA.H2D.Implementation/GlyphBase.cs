using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for GlyphBase.
	/// </summary>
	public abstract class GlyphBase :IGlyph
	{
		public GlyphBase()
		{
		}

		public GlyphBase (string id)
		{
			_Id = id;
		}

		#region IGlyph Members

		protected ArrayList _Children = new ArrayList ();

		public IEnumerable Children 
		{
			get
			{
				return _Children;
			} 
		}
 
		public void AddChild (IGlyph child)
		{
			if (!_Children.Contains (child))
			{
				_Children.Add (child);
				// must be after _Children.Add since child.Parent = parent 
				// calls back to parent.AddChild (child)
				child.Parent = this;
			}
		}

		public void RemoveChild (IGlyph child)
		{
			if (_Children.Contains (child))
			{
				_Children.Remove (child);
			}
		}

		protected ArrayList _OwnedItems = new ArrayList ();

		public IEnumerable OwnedItems 
		{
			get
			{
				return _OwnedItems;
			} 
		}

		public void AddOwned (IGlyph owned)
		{
			if (!_OwnedItems.Contains (owned))
			{
				_OwnedItems.Add (owned);
				// must be after _OwnedItems.Add since owned.Owner = owner 
				// calls back to owner.AddOwned (owned)
				owned.Owner = this;
			}
		}

		public void RemoveOwned (IGlyph owned)
		{
			if (_OwnedItems.Contains (owned))
			{
				_OwnedItems.Remove (owned);
			}
		}

		string _Id = Guid.NewGuid ().ToString ();

		[Category ("Identity")]
		public string Id
		{
			get
			{
				return _Id;
			}
		}

		string _Name;
		[Category ("Identity")]
		public string Name { get { return _Name; } set { _Name = value; } }

		public event MurphyPA.H2D.Interfaces.OffsetChangedHandler OffsetChanged;


		protected IGlyph _Owner;
		public IGlyph Owner 
		{ 
			get 
			{
				return _Owner; 
			} 
			set 
			{
				System.Diagnostics.Debug.Assert (value != this);

				if (_Owner == value)
				{
					return;
				}

				if (_Owner != null)
				{
					_Owner.RemoveOwned (this);
				}
				_Owner = value; 
				if (_Owner != null)
				{
					_Owner.AddOwned (this);
				}
			}
		}

		protected IGlyph _Parent;

		public IGlyph Parent 
		{ 
			get 
			{
				return _Parent;
			}
			set
			{
				System.Diagnostics.Debug.Assert (value != this);

				if (_Parent == value)
				{
					return;
				}

				if (_Parent != null)
				{
					_Parent.RemoveChild (this);
				}
				_Parent = value;
				if (_Parent != null)
				{
					_Parent.AddChild (this);
				}
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return GetBounds ();
			}
		}

		bool _Selected;
		public bool Selected 
		{ 
			get { return _Selected; }
			set { _Selected = value; }
		}

		protected virtual void OnBeforeOffsetChanged (OffsetEventArgs offsetArgs)
		{
		}

		protected virtual bool OnOffsetChanged (OffsetEventArgs offsetArgs)
		{
			return true;
		}

		protected void DoOffsetChanged (OffsetEventArgs offsetArgs)
		{
			if (OnOffsetChanged (offsetArgs))
			{
				if (OffsetChanged != null)
				{
					OffsetChanged (this, offsetArgs);
				}
			}
		}


		public virtual bool ContainsPoint(System.Drawing.Point point)
		{
			return Bounds.Contains (point);
		}

		public virtual void Accept (IGlyphVisitor visitor)
		{
			visitor.Visit (this);
		}

		protected bool IsNotEmptyString (string s)
		{
			return s != null && s.Trim () != "";
		}

		protected string GlyphNameFromString (string name)
		{
			if (IsNotEmptyString (name))
			{
				return name;
			}
			return "NOT_NAMED";
		}

		protected virtual bool AcceptParentInFullyQualifiedName (IGlyph glyph)
		{
			return true;
		}

		protected string GlyphNameFrom (IGlyph glyph)
		{
			Stack stack = new Stack ();
			while (glyph != null)
			{
				stack.Push (glyph);
				glyph = glyph.Parent;
				while (glyph != null && AcceptParentInFullyQualifiedName (glyph) == false)
				{
					glyph = glyph.Parent;
				}
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			bool first = true;
			while (stack.Count > 0)
			{
				if (first)
				{
					first = false; 
				} 
				else
				{
					builder.Append (".");
				}
				glyph = stack.Pop () as IGlyph;
				builder.Append (GlyphNameFromString (glyph.Name));
			}
			return builder.ToString ();
		}

		[Category ("Identity")]
		public string FullyQualifiedStateName 
		{
			get 
			{
				return GlyphNameFrom (this);
			}
		}

		string _Note;
		[Category ("Identity")]
		public string Note 
		{ 
			get { return _Note; } 
			set { _Note = value; }
		}


		protected abstract Rectangle GetBounds ();
		public abstract void MoveTo(Point point);
		public abstract void Offset(Point point);
		public abstract void Draw(IGraphicsContext GC);

		bool _DoNotInstrument;
		[Category ("Debugging")]
		public bool DoNotInstrument { get { return _DoNotInstrument; } set { _DoNotInstrument = value; } }

		#endregion
	}
}
