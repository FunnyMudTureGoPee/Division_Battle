using System;

namespace Script.Battalion
{
    public class Buff
    {
        private BattalionData _battalionData;
        public string funName { get; set; }
        private object[] parameters = null;

        // 初始执行事件
        public Buff(string funName)
        {
            this.funName = funName;
        }

        public Buff(string funName, object[] parameters = null)
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
        public Buff(BattalionData battalionData, string funName, object[] parameters = null)
        {
            this._battalionData = battalionData;
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

        public void Buff_test(BattalionData battalionData)
        {
            battalionData._Hp += 0.05;
        }

        ////////
        /// 相邻buff系列
        /// 格式 “Buff_受益者类型_来源者类型”
        /// <summary>
        /// 人海攻势
        /// </summary>
        public void Buff_Infantry_Infantry(BattalionData battalionData)
        {
            battalionData._ReOp += 0.05;
        }

        /// <summary>
        /// 前线火力 步兵-火炮
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Infantry_Artillery(BattalionData battalionData)
        {
            battalionData._Def += 0.05;
            battalionData._Att += 0.05;
        }

        /// <summary>
        /// 步坦协同 步兵-坦克
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Infantry_Armor(BattalionData battalionData)
        {
            battalionData._Att += 0.05;
            battalionData._Def += 0.02;
            battalionData._ReOp += 0.01;
        }

        /// <summary>
        /// 炮兵前观 炮兵-步兵
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Artillery_Infantry(BattalionData battalionData)
        {
            battalionData._Att += 0.1;
            battalionData._Def += 0.1;
        }

        /// <summary>
        /// 集中火力 炮兵-炮兵
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Artillery_Artillery(BattalionData battalionData)
        {
            battalionData._Att += 0.2;
            battalionData._Def -= 0.1;
            battalionData._ReOp -= 0.05;
        }
        /// <summary>
        /// 火力支援 炮兵-装甲
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Artillery_Armor(BattalionData battalionData)
        {
            battalionData._Att -= 0.2;
            battalionData._Def -= 0.1;
            battalionData._ReOp -= 0.05;
        }
        
        /// <summary>
        /// 步坦协同 装甲-步兵
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Armor_Infantry(BattalionData battalionData)
        {
            battalionData._Def += 0.05;
            battalionData._ReOp += 0.1;
        }
        
        /// <summary>
        /// 火力支援 装甲-炮兵
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Armor_Artillery(BattalionData battalionData)
        {
            battalionData._Att += 0.2;
            battalionData._Def += 0.1;
        }
        
        

        /// <summary>
        /// 装甲矛头 装甲-装甲
        /// </summary>
        /// <param name="battalionData"></param>
        public void Buff_Armor_Armor(BattalionData battalionData)
        {
            battalionData._Att += 0.1;
            battalionData._ReOp += 0.1;
        }
        
        ////////////////////////////////////////////////////////////////
        /// 特殊buff
        ////////////////////////////////////////////////////////////////

        public void Buff_LowOp(BattalionData battalionData)
        {
            battalionData._Att *= 0.1;
            battalionData._Def *= 0.1;
        }
    }
}