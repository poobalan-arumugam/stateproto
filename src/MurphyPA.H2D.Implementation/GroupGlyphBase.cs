using System;
using System.Drawing;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.Implementation
{
	/// <summary>
	/// Summary description for GroupGlyphBase.
	/// </summary>
	public abstract class GroupGlyphBase : GlyphBase, IGroupGlyph
	{
		public GroupGlyphBase()
		{
		}

		public GroupGlyphBase(string id)
			: base (id)
		{
		}

		#region ICompositeGlyph Members

		public bool ContainsGlyph(IGlyph glyph)
		{
			return false;
		}

		#endregion

		#region IGroupGlyph Members

		protected ArrayList _ContactPoints = new ArrayList ();

		public IEnumerable ContactPoints
		{
			get
			{
				return _ContactPoints;
			}
		}

		#endregion

		protected virtual void ReOffsetContactPoint (IGlyph contact, Point point)
		{
			contact.OffsetChanged -= new OffsetChangedHandler (contactPoint_OffsetChanged);
			contact.Offset (point);
			contact.OffsetChanged += new OffsetChangedHandler (contactPoint_OffsetChanged);
		}

		protected virtual void AddContactPoint (IGlyph contactPoint)
		{
			contactPoint.Owner = this;
			_ContactPoints.Add (contactPoint);
			contactPoint.OffsetChanged += new OffsetChangedHandler(contactPoint_OffsetChanged);
		}

		protected abstract void contactPoint_OffsetChanged(IGlyph glyph, OffsetEventArgs offsetEventArgs);
	}
}
