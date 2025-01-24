
/// <summary>
/// Part of a combo chain.
/// </summary>
public class ComboPart
{
    public string part;
    public bool waitingForInput;
    public int anim_hash;
    public int index;


    public ComboPart(string _param, bool _waitForInput, int _anim_hash, int _index)
    {

        part = _param;
        waitingForInput = _waitForInput;
        anim_hash = _anim_hash;
        index = _index;
    }

    public string PartName
    {
        get
        {
            return part;
        }

        set
        {
            part = value;
        }
    }
}
