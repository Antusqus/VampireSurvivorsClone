using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellcast : Dictionary<string, KeyCode>
{

    Dictionary<string, KeyCode> spellBook;

    public PlayerSpellcast(KeyCode _button, string _name)
    {

        spellBook[_name] = _button;
    }
}
