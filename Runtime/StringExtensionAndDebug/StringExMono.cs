using UnityEngine;
using static StringEx;


public class StringExMono : MonoBehaviour
{
    public int value = 10;
    void Start()
    {
        /// can be done like so with normal debug.log but use LOG instead as then you can turn off LOG with single Bool Key for production for optimization
        ///  StringEx.DoLog = false; /* To use with turning off Log globally ,but can be used within section. can be confusing thou so don't do within scripts.*/

        Debug.Log($" IsSaving :C:b:18; hello {Apply(Color.red, FontStyle.Bold, value)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.yellow, FontStyle.BoldAndItalic, value)}".Interpolate());
        Debug.Log($" IsSaving :white:i; hello :BLACK:{FontStyle.BoldAndItalic}:19; Thus ReStarting :i; SaveRoutine  :yellow:10;".Interpolate());
        Debug.Log($" IsSaving TiTLeCase :W:I:15:T; hello :R:BI:19; Thus ReStarting :i;  \"SaveRoutine\" :M:10:I;".Interpolate());
        Debug.Log($" IsSaving :{Color.gray}:b; hello {Apply("Bl", "BI", 25)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.yellow, FontStyle.BoldAndItalic, value)}".Interpolate());

        // install https://marketplace.visualstudio.com/items?itemName=NikolaMSFT.InlineColorPicker if you wish to see preview of color

        Log('*'); // draws  * Line
        Log($" NoTrace :C:b:18; hello {Apply(Color.red, FontStyle.Bold, value)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.green, FontStyle.BoldAndItalic, value)}");

        Log($"tesTing bAd Case HELLO :G:19:Bi:T; ");
        Log($"tesTing :{C.brown}:T; SuPerbAd :G:19:Bi:t; CaSe :B:{Variant.TitleCase}; HELLO :{Color.magenta}:19:{FontStyle.Bold}:T;");

        LogError($" NoTrace :white:i; hello :BLACK:{FontStyle.BoldAndItalic}:19; Thus ReStarting :i; SaveRoutine  :yellow:10;");
        LogWarning($" NoTrace :W:I:15; hello :R:BI:19; Thus ReStarting :i;  \"SaveRoutine\" :M:10:I;");
        LogAssert($" NoTrace :{Color.gray}:b; hello {Apply("Bl", "BI", 25)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.magenta, FontStyle.BoldAndItalic, value)}");



        Log(Color.green); // draws  line of color

        LogT($" WithTrace :C:b:10:I; HELLO {Apply(Color.red, FontStyle.Bold, value)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.yellow, FontStyle.BoldAndItalic, value)}");
        LogErrorT($" WithTrace :white:i; hello :BLACK:{FontStyle.BoldAndItalic}:19; Thus ReStarting :i; SaveRoutine  :yellow:10;");
        LogWarningT($" WithTrace :W:I:15; HELLO :R:BI:19:l; Thus ReStarting :i;  \"SaveRoutine\" :M:10:I;");
        LogAssertT($" WithTrace :{Color.gray}:b; hello {Apply("Bl", "BI", 25)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.magenta, FontStyle.BoldAndItalic, value)}");


        Log(Color.blue, '+', 200); // draws color line of any char  with of defined length


        Log($" NoTrace :C:b:18; Hello {Apply(Color.red, FontStyle.Bold, value)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.green, FontStyle.BoldAndItalic, value)}");

        LogT($"Trace :C:b:18; Hello {Apply(Color.red, FontStyle.Bold, value)} Thus ReStarting {Apply("green,bi")}   \"SaveRoutine\"{Apply(Color.green, FontStyle.BoldAndItalic, value)}");


        // Log/LogError/LogWarning/LogAssert doesn't do trace (if all you want is to get clean debug) ; LogT/LogErrorT/LogWarningT/LogAssertT does trace that you can logTrace to script

        // this is a string Extension thus we can use it for GUI / tesxmeshpro or text also
    }


}
