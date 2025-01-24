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
