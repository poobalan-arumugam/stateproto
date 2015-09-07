using System;
using System.Collections;
using MurphyPA.H2D.Interfaces;

namespace MurphyPA.H2D.TestApp
{
	/// <summary>
	/// Summary description for DiagramModel.
	/// </summary>
	public class DiagramModel
	{
		ArrayList _Glyphs = new ArrayList ();
		ArrayList _DeletedGlyphs = new ArrayList ();
		StateMachineHeader _Header = new StateMachineHeader ();

		public IEnumerable Glyphs { get { return _Glyphs; } }
		public StateMachineHeader Header { get { return _Header; } }

		public ArrayList GetGlyphsList () { return _Glyphs; }

		public DiagramModel()
		{
		}

		public DiagramModel (StateMachineHeader header, ArrayList glyphs)
		{
			_Header = header;
			_Glyphs = glyphs;
		}

		public bool HasGlyphs ()
		{
			return _Glyphs.Count > 0;
		}

		bool _IsDirty;
		public bool IsDirty 
		{
			get 
			{
				return _IsDirty;
			}
			set 
			{
				_IsDirty = value;
			}
		}

		public void AddGlyph (IGlyph glyph)
		{
			_Glyphs.Add (glyph);
			_IsDirty = true;
		}

		public void DeSelectAllGlyphs ()
		{
			foreach (IGlyph glyph in _Glyphs)
			{
				glyph.Selected = false;
			}
		}

		IGlyph InnerFindGlyph (IGlyph glyph, System.Drawing.Point point)
		{
			foreach (IGlyph child in glyph.Children)
			{
				if (child.ContainsPoint (point))
				{
					if (ContainsGlyph (child))
					{
						return InnerFindGlyph (child, point);
					}
				}
			}
			return glyph;
		}

		IGlyph InnerFindGlyph (System.Drawing.Point point)
		{
			foreach (IGlyph glyph in _Glyphs)
			{
				if (glyph.ContainsPoint (point))
				{
					return InnerFindGlyph (glyph, point);
				}
			}
			return null;
		}

		IGlyph InnerFindContactPoint (System.Drawing.Point point, out IGlyph parent)
		{
			parent = null;
			foreach (IGlyph glyph in _Glyphs)
			{
				IGroupGlyph groupGlyph = glyph as IGroupGlyph;
				if (groupGlyph != null)
				{
					foreach (IGlyph contact in groupGlyph.ContactPoints)
					{
						if (contact.ContainsPoint (point))
						{
							parent = groupGlyph;
							return contact;
						}
					}
				}
			}
			return null;
		}

		public IGlyph FindGlyph (System.Drawing.Point point)
		{
			IGlyph glyph = InnerFindGlyph (point);
			if (ContainsDeletedGlyph (glyph))
			{
				System.Diagnostics.Debug.Assert (false, "Deleted glyph found in _Glyphs or glyph.Children - see InnerFindGlyph ()");
			}
			return glyph;
		}

		public IGlyph FindContactPoint (System.Drawing.Point point, out IGlyph parent)
		{
			IGlyph glyph = InnerFindContactPoint (point, out parent);
			if (ContainsDeletedGlyph (glyph))
			{
				System.Diagnostics.Debug.Assert (false, "Deleted glyph found in _Glyphs or glyph.ContactPoints - see InnerFindContactPoint ()");
			}
			if (ContainsDeletedGlyph (parent))
			{
				System.Diagnostics.Debug.Assert (false, "Deleted glyph found in _Glyphs or glyph.Children - see InnerFindContactPoint ()");
			}
			return glyph;
		}

		public bool IsStateGlyph (IGlyph glyph)
		{
			return glyph is IStateGlyph;
		}

		public bool IsTransitionGlyph (IGlyph glyph)
		{
			return glyph is ITransitionGlyph;
		}

		public bool IsStateTransitionPortGlyph (IGlyph glyph)
		{
			return glyph is IStateTransitionPortGlyph;
		}

		public bool IsTransitionContactPointGlyph (IGlyph glyph)
		{
			return glyph is ITransitionContactPointGlyph;
		}

		public bool IsComponentGlyph (IGlyph glyph)
		{
			return glyph is IComponentGlyph;
		}

		public bool IsOperationGlyph (IGlyph glyph)
		{
			return glyph is IOperationGlyph;
		}

		public IGlyph FindInnerMostChildContainingBound (IGlyph glyph, System.Drawing.Rectangle bound, IsSupportedGlyphHandler isSupported)
		{
			return FindInnerMostChildContainingBound (glyph, bound, isSupported, null);
		}

		public IGlyph FindInnerMostChildContainingBound (IGlyph glyph, System.Drawing.Rectangle bound, IsSupportedGlyphHandler isSupported, IGlyph excludeGlyph)
		{
			foreach (IGlyph child in glyph.Children)
			{
				if (!isSupported (child))
				{
					continue;
				}
				if (child == excludeGlyph)
				{
					continue;
				}
				if (child.Bounds.Contains (bound))
				{
					return FindInnerMostChildContainingBound (child, bound, isSupported);
				}
			}
			return glyph;
		}

		public void ReparentGlyphs (IGlyph selectedGlyph, IEnumerable glyphs, IsSupportedGlyphHandler isSupported)
		{
			selectedGlyph.Parent = null;
			foreach (IGlyph glyph in glyphs)
			{
				if (!isSupported (glyph))
				{
					continue;
				}

				if (glyph.Parent != null)
				{
					if (!glyph.Parent.Bounds.Contains (glyph.Bounds))
					{
						glyph.Parent = null;
					} 
				}
				if (glyph != selectedGlyph)
				{
					if (glyph.Bounds.Contains (selectedGlyph.Bounds))
					{
						if (selectedGlyph.Parent == null)
						{
							selectedGlyph.Parent = glyph;
						} 
						else if (selectedGlyph.Parent.Bounds.Contains (glyph.Bounds))
						{
							selectedGlyph.Parent = glyph;
						}
					} 
					else if (glyph.Parent == null && selectedGlyph.Bounds.Contains (glyph.Bounds))
					{
						glyph.Parent = selectedGlyph;
					}
				}
			}

			bool changed = true;
			while (changed) 
			{
				changed = false;
				foreach (IGlyph glyph in glyphs)
				{
					if (!isSupported(glyph))
					{
						continue;
					}

					if (glyph.Parent != null)
					{
						if (glyph.Parent.Bounds.Contains (glyph.Bounds))
						{
							foreach (IGlyph child in glyph.Parent.Children)
							{
								if (!(glyph is IStateGlyph))
								{
									continue;
								}

								if (child != glyph)
								{
									if (child.Bounds.Contains (glyph.Bounds))
									{
										glyph.Parent = child;
										changed = true;
										break;
									}
								}
							}
						} 
					}
				}
			}
		}

		public bool HasGlyphElements (IsSupportedGlyphHandler handler)
		{
			foreach (IGlyph glyph in _Glyphs)
			{
				if (handler (glyph))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasComponentFrameElements ()
		{
			return HasGlyphElements (new IsSupportedGlyphHandler (IsComponentGlyph));
		}

		public bool HasStateElements ()
		{
			return HasGlyphElements (new IsSupportedGlyphHandler (IsStateGlyph));
		}

		public void RemoveGlyph (IGlyph glyph)
		{
			_Glyphs.Remove (glyph);
			_DeletedGlyphs.Add (glyph);
		}

		public bool ContainsGlyph (IGlyph glyph)
		{
			return _Glyphs.Contains (glyph);
		}

		public bool ContainsDeletedGlyph (IGlyph glyph)
		{
			return _DeletedGlyphs.Contains (glyph);
		}

		public System.Drawing.Rectangle GetDiagramBounds ()
		{
			int right = 0;
			int bottom = 0;
			foreach (IGlyph glyph in _Glyphs)
			{
				System.Drawing.Rectangle bound = glyph.Bounds;
				right = Math.Max (right, bound.Right);
				bottom = Math.Max (bottom, bound.Bottom);
			}

			int left = right;
			int top = bottom;
			foreach (IGlyph glyph in _Glyphs)
			{
				System.Drawing.Rectangle bound = glyph.Bounds;
				left = Math.Min (left, bound.Left);
				top = Math.Min (top, bound.Top);
			}
			left -= 15;
			top -= 15;

			right += 15;
			bottom += 15;

			System.Drawing.Rectangle bounds = new System.Drawing.Rectangle (left, top, right - left, bottom - top);
			return bounds;
		}

#warning need to separate parenting as hierarchical containment from ownership (contactpoints are owned by one glyph but could be parented by another - aka, TransitionContactPointCircleGlyph). Glyph has .Owner to define this - but has this been correctly assigned where it needs to be?
		public void DeleteSelectedGlyphs ()
		{
			ArrayList glyphList = new ArrayList ();
			foreach (IGlyph glyph in _Glyphs)
			{
				glyphList.Add (glyph);
			}
			foreach (IGlyph glyph in glyphList)
			{
				if (glyph != null && glyph.Selected)
				{
					IGlyph parent = glyph.Parent;

					// reparent children to parent's parent
					ArrayList children = new ArrayList ();
					foreach (IGlyph child in glyph.Children)
					{
						if (glyph != child.Owner)
						{
							children.Add (child);
						}
					}
					foreach (IGlyph child in children)
					{
						child.Parent = parent;
					}

					// unparent owned items
					ArrayList ownedItems = new ArrayList ();
					foreach (IGlyph ownedItem in glyph.OwnedItems)
					{
						if (glyph != ownedItem.Parent)
						{
							ownedItems.Add (ownedItem);
						}
					}
					foreach (IGlyph ownedItem in ownedItems)
					{
						ownedItem.Parent = null;
					}

					System.Diagnostics.Debug.Assert (ContainsGlyph (glyph));
					RemoveGlyph (glyph);
					glyph.Parent = null;
				}
			}
		}
	}

	public delegate bool IsSupportedGlyphHandler (IGlyph glyph);

}
