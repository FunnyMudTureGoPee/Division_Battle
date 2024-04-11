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
        
        public object Run(bool isReturn ,object[] parameters = null)
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
            return 1;
        }
        
        public int Guerrilla(OutputData outputData)
        {
            outputData.ATT *= 0.25;
            outputData.DEF *= 2;
            return 2;
        }

        //////////////////////////////////////////////////////////////////
        ///防御战术卡 组织度恢复提升，防御提升
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// 基础防御 
        /// </summary>
        /// <param name="outputData"></param>
        /// <returns></returns>
        public int  Defend(OutputData outputData)
        {
            outputData.ReOP *= 1.2;
            outputData.DEF *= 1.2;
            return 1;
        }
    }
}