using System;
using System.Collections.Generic;
using UnityEngine;


public class ComboChain
{
	Dictionary<string, ComboPart> _comboChain;

	public Dictionary<string, ComboPart> ComboChainDict
    {
        get
        {
            return _comboChain;
        }
        set
        {
            _comboChain = value;
        }
    }   // Class for full combo chain.
    public ComboChain(string comboName, List<ComboPart> comboParts)
	{
		foreach (ComboPart comboPart in comboParts)
        {
			_comboChain[comboName] = comboPart;
        }
	}
}

/// <summary>
/// Part of a combo chain.
/// </summary>
public class ComboPart
{
	public string part;
	public bool waitForInput;
    public int anim_hash;


    public ComboPart(string _param, bool _waitForInput, int _anim_hash)
	{
        
		part = _param;
		waitForInput = _waitForInput;
        anim_hash = _anim_hash;
	}

	public string PartName
    {
        get {
			return part;
		}

        set
        {
			part = value;
        }
    }
}
