using System.Collections.Generic;

public enum SpecialParam
{
    SpecialParamMin = 10000000,
    
    Expression,             //                          用于标记表达式Expression开始
    End,                    //                          用于标记表达式Expression结束

    //...
    
    SpecialParamMax
}

public static class SpecialParamConvert
{
    public static void ConvertParam(List<int> param)
    {
        for (var i = param.Count - 1; i >= 0; i--)
        {
            if (param[i] is <= (int)SpecialParam.SpecialParamMin or >= (int)SpecialParam.SpecialParamMax) continue;
            switch ((SpecialParam)param[i])
            {
                case SpecialParam.Expression:
                    var length = param.IndexOf((int)SpecialParam.End, i + 1) - i - 1;
                    param[i] = ExpressionEvaluator.Evaluate(param.GetRange(i + 1, length));
                    param.RemoveRange(i + 1, length + 1);
                    break;
            }
        }
    }
}