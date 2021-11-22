using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class Attributes
    {
        public AttributeData Initial = new AttributeData();
        public AttributeData Growth = new AttributeData();
        public AttributeData Equip = new AttributeData();
        public AttributeData Basic = new AttributeData();
        public AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();

        int Level;

        public NAttributeDynamic DynamicAttr;

        public float HP
        {
            get { return DynamicAttr.Hp; }
            set { DynamicAttr.Hp = (int)Math.Min(MaxHP, value); }
        }
        public float MP
        {
            get { return DynamicAttr.Mp; }
            set { DynamicAttr.Mp = (int)Math.Min(MaxMP, value); }
        }

        /// <summary>
        /// 最大生命
        /// </summary>
        public float MaxHP { get { return this.Final.MaxHP; } }

        /// <summary>
        /// 最大法力
        /// </summary>
        public float MaxMP { get { return this.Final.MaxMP; } }

        /// <summary>
        /// 力量
        /// </summary>
        public float STR { get { return this.Final.STR; } }

        /// <summary>
        /// 智力
        /// </summary>
        public float INI { get { return this.Final.INT; } }

        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX { get { return this.Final.INT; } }

        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD { get { return this.Final.AD; } }

        /// <summary>
        /// 法术攻击
        /// </summary>
        public float AP { get { return this.Final.AP; } }

        /// <summary>
        /// 物理防御
        /// </summary>
        public float DEF { get { return this.Final.DEF; } }

        /// <summary>
        /// 法术防御
        /// </summary>
        public float MDEF { get { return this.Final.MDEF; } }

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float SPD { get { return this.Final.SPD; } }

        /// <summary>
        /// 暴击概率
        /// </summary>
        public float CRI { get { return this.Final.CRI; } }

        /// <summary>
        /// 初始化角色属性
        /// </summary>
        public void Init(CharacterDefine define, int level, List<EquipDefine> equips, NAttributeDynamic dynamicAttr)
        {
            this.DynamicAttr = dynamicAttr;
            this.LoadInitAttribute(this.Initial, define);
            this.LoadGrowthAttribute(this.Growth, define);
            this.LoadEquipAttribute(this.Equip, equips);
            this.Level = level;
            this.InitBasicAttributes();
            this.InitSecondaryAttributes();

            this.InitFinalAttributer();

            if (this.DynamicAttr == null)
            {
                this.DynamicAttr = new NAttributeDynamic();
                this.HP = this.MaxHP;
                this.MP = this.MaxMP;
            }
            else
            {
                this.HP = this.DynamicAttr.Hp;
                this.MP = this.DynamicAttr.Mp;
            }
        }

        /// <summary>
        /// 计算基础属性
        /// </summary>
        public void InitBasicAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i];
            }

            for (int i = (int)AttributeType.STR; i <= (int)AttributeType.DEX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i] + this.Growth.Data[i] * (this.Level - 1);//一级属性成长
                this.Basic.Data[i] += this.Equip.Data[i];//装备一级属性加成在计算属性前
            }
        }
        /// <summary>
        /// 计算二级属性
        /// </summary>
        public void InitSecondaryAttributes()
        {
            //二级属性成长（包括装备）
            this.Basic.MaxHP = this.Basic.STR * 10 + this.Initial.MaxHP + this.Equip.MaxHP;
            this.Basic.MaxMP = this.Basic.INT * 10 + this.Initial.MaxMP + this.Equip.MaxMP;

            this.Basic.AD = this.Basic.STR * 5 + this.Initial.AD + this.Equip.AD;
            this.Basic.AP = this.Basic.INT * 5 + this.Initial.AP + this.Equip.AP;
            this.Basic.DEF = this.Basic.STR * 2 + this.Basic.DEX * 1 + this.Initial.DEF + this.Equip.DEF;
            this.Basic.MDEF = this.Basic.INT * 2 + this.Basic.DEX * 1 + this.Initial.MDEF + this.Equip.MDEF;

            this.Basic.SPD = this.Basic.DEX * 0.2f + this.Initial.SPD + this.Equip.SPD;
            this.Basic.CRI = this.Basic.DEX * 0.0002f + this.Initial.CRI + this.Equip.CRI;

        }

        public void InitFinalAttributer()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Final.Data[i] = this.Basic.Data[i] + this.Buff.Data[i];
            }
        }

        /// <summary>
        /// 加载初始属性
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="define"></param>
        public void LoadInitAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.MaxHP = define.MaxHP;
            attr.MaxMP = define.MaxMP;
            attr.STR = define.STR;
            attr.INT = define.INT;
            attr.DEX = define.DEX;
            attr.AD = define.AD;
            attr.AP = define.AP;
            attr.DEF = define.DEF;
            attr.MDEF = define.MDEF;
            attr.SPD = define.SPD;
            attr.CRI = define.CRI;

        }
        /// <summary>
        /// 加载成长属性
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="define"></param>
        public void LoadGrowthAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.STR = define.GrowthSTR;
            attr.INT = define.GrowthINT;
            attr.DEX = define.GrowthDEX;
        }

        /// <summary>
        /// 加载装备属性
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="define"></param>
        public void LoadEquipAttribute(AttributeData attr, List<EquipDefine> equips)
        {
            attr.Reset();
            if (equips == null) return;
            foreach (var define in equips)
            {
                attr.MaxHP += define.MaxHP;
                attr.MaxMP += define.MaxMP;
                attr.STR += define.STR;
                attr.INT += define.INT;
                attr.DEX += define.DEX;
                attr.AD += define.AD;
                attr.AP += define.AP;
                attr.DEF += define.DEF;
                attr.MDEF += define.MDEF;
                attr.SPD += define.SPD;
                attr.CRI += define.CRI;
            }
        }

    }
}
