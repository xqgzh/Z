using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Z.Util
{
    static class RefectingTools
    {
        public static string GetInterfaceName()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                StackTrace st = new StackTrace();
                

                #region 获取Web方法名称


                if (st.FrameCount > 1)
                {
                    for (int i = st.FrameCount; i > 1; i--)
                    {
                        StackFrame sf = st.GetFrame(i);

                        if (sf != null)
                        {
                            MethodBase mi = sf.GetMethod();

                            if(mi != null && mi.DeclaringType != null && mi.DeclaringType.Assembly != null && mi.DeclaringType.Assembly.GlobalAssemblyCache == false)
                            {
                                if (mi.DeclaringType != typeof(Z.Log.Logger))
                                {
                                    if (sb.Length > 0)
                                    {
                                        sb.Append("->");
                                    }
                                    
                                    sb.Append(mi.DeclaringType.Name);
                                    sb.Append('.');
                                    sb.Append(mi.Name);
                                }
                            }
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                sb.Append(ex.Message);
            }

            return sb.ToString();

        }
    }
}
