using System;
using UnityEngine;

namespace Script
{
    public class Buff
    {
        private BattalionData _battalionData;
        private string funName{ get;  set; }
        private object[] parameters = null;
        
        // 初始执行事件
        public Buff(string funName)
        {
            this.funName = funName;
        }
        
        public Buff(string funName,object[] parameters=null)
        {
            this.funName = funName;
            this.parameters =parameters;
        }
        /// <summary>
        /// 带有绑定对象的buff
        /// </summary>
        /// <param name="battalionData">绑定对象</param>
        /// <param name="funName">方法名</param>
        /// <param name="parameters">方法参数列表</param>
        public Buff(BattalionData battalionData, string funName, object[] parameters = null)
        {
            this._battalionData = battalionData;
            this.funName = funName;
            this.parameters =parameters;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="funName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <exception cref="NullReferenceException">方法名不存在</exception>

        public void Run(string funName,object[] parameters=null)
        {
            var type = typeof(Buff);
            var method = type.GetMethod(funName);
            if (method == null)
                throw new NullReferenceException("方法" + funName + "不存在");
            Buff obj = new Buff(funName);
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
            var type = typeof(Buff);
            var method = type.GetMethod(funName);
            if (method == null)
                throw new NullReferenceException("方法" + funName + "不存在");
            Buff obj = new Buff(funName);
            //执行方法
            method.Invoke(obj, this.parameters ?? parameters);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void VoidBuff()
        {
            
        }
    }
}