namespace NLEditor
{
  // This is NOT a correct implementation. It's bare minimum to get sane results in the editor.

  class LoadStyleAnimData
  {
    public string Name;
    public int Frames;
    public bool HorizontalStrip;
    public int ZIndex;
    public int OffsetX;
    public int OffsetY;
    public int Frame; // -1 for match primary
  }
}
