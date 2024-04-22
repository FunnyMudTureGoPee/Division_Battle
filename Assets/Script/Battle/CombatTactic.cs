using System;
using Script.Battalion;

namespace Script
{
    public class CombatTactic
    {
        private OutputData OutputData;
        private int hit;
        public string funName { get; set; }
        private object[] parameters = null;

        // 初始执行事件
        public CombatTactic(string funName)
        {
            this.funName = funName;
        }

        public CombatTactic(string funName, object[] parameters = null)
        {
            this.funName = funName;
            this.parameters = parameters;
        }

        /// <summary>
        /// 带有绑定对象的buff
        /// </summary>
        /// <param name="battalionData">绑定对象</param>
        /// <param name="funName">方法名</param>
        /// <param name="parameters">方法参数列表</param>
        public CombatTactic(OutputData OutputData, string funName, object[] parameters = null)
        {
            this.OutputData = OutputData;
            this.funName = funName;
            this.parameters = parameters;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="funName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <exception cref="NullReferenceException">方法名不存在</exception>
        public void Run(string funName, object[] parameters = null)
        {
            var type = typeof(CombatTactic);
            var method = type.GetMethod(funName);
            if (method == null)
                throw new NullReferenceException("方法" + funName + "不存在");
            CombatTactic obj = new CombatTactic(funName);
            //执行方法
            method.Invoke(obj, this.parameters ?? parameters);
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="funName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <exception cref="NullReferenceException">方法名不存在</exception>
        public void Run(object[] parameters = null)
        {
            var type = typeof(CombatTactic);
            var method = type.GetMethod(funName);
            if (method == null)
                throw new NullReferenceException("方法" + funName + "不存在");
            CombatTactic obj = new CombatTactic(funName);
            //执行方法
            method.Invoke(obj, this.parameters ?? parameters);
        }

        public object Run(bool isReturn, object[] parameters = null)
        {
            var type = typeof(CombatTactic);
            var method = type.GetMethod(funName);
            if (method == null)
                throw new NullReferenceException("方法" + funName + "不存在");
            CombatTactic obj = new CombatTactic(funName);
            //执行方法
            return method.Invoke(obj, this.parameters ?? parameters);
        }

        //////////////////////////////////////////////////////////////////
        /// 攻击战术卡
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// 基础攻击
        /// </summary>
        /// <param name="outputData"></param>
        /// <returns></returns>
        public int Attack(OutputData outputData)
        {
            var ATTtemp = outputData.ATT;
            var DEFtemp = outputData.DEF;
            outputData.ATT = ATTtemp * 0.8 + DEFtemp * 0.2;
            outputData.DEF = ATTtemp * 0.3 + DEFtemp * 0.2;
            return 1;
        }

        public int Guerrilla(OutputData outputData)
        {
            var ATTtemp = outputData.ATT;
            var DEFtemp = outputData.DEF;
            outputData.ATT = ATTtemp * 0.1 + DEFtemp * 0.15;
            outputData.DEF = DEFtemp * 1.5 + ATTtemp * 0.25;
            return 3;
        }

        //////////////////////////////////////////////////////////////////
        ///防御战术卡 防御提升
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// 基础防御 
        /// </summary>
        /// <param name="outputData"></param>
        /// <returns></returns>
        public int Defend(OutputData outputData)
        {
            var ATTtemp = outputData.ATT;
            var DEFtemp = outputData.DEF;
            outputData.ATT = ATTtemp * 0.2 + DEFtemp * 0.3;
            outputData.DEF = ATTtemp * 0.2 + DEFtemp * 0.8;
            return 1;
        }

        /// <summary>
        /// 基础防御 
        /// </summary>
        /// <param name="outputData"></param>
        /// <returns></returns>
        public int Default(OutputData outputData)
        {
            outputData.DEF *= 0.9;
            outputData.ATT *= 0.9;
            return 1;
        }
    }
}