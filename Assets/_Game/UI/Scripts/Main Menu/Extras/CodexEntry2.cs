using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexEntry2 : CodexEntry
{
    [TextArea(0, 100)]
    public string paragraphTextLeft;

    [TextArea(0, 100)]
    public string extraNotesBottom;

    public Sprite imageTopRight;
}
