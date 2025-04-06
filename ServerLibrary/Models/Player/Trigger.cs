using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //成就触发
    {
        /*
         * 触发中的状态类要求必须满足 才判断其他条件
         */
        public bool TriggerConditionSatisfied(TriggerInfo trigger) //是否可以更新成就进度
        {
            foreach (TriggerCondition condition in trigger.TriggerConditions)
            {
                switch (condition.ConditionType)
                {
                    case TriggerConditionType.InMap:
                        if (condition.MapParameter != null && CurrentMap.Info.Index != condition.MapParameter.Index)
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.WearingItem:
                        if (condition.ItemParameter != null && !WearingItem(condition.ItemParameter))
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.CarryingItem:
                        if (condition.ItemParameter != null && GetItemCount(condition.ItemParameter) < 1)
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.LevelLessThan:
                        if (Level > condition.RequiredAmount)
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.LevelGreaterOrEqualThan: //大于等于某级别
                        if (Level <= condition.RequiredAmount)
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.UseMagic: //TODO 需要测试
                        if (CurrentMagic != condition?.MagicParameter?.Magic)
                        {
                            return false;
                        }
                        break;
                    case TriggerConditionType.TimeConstraint: //TODO 需要测试
                        {
                            int setDay = condition.TimeParameter.Days;
                            int today = ((int)SEnvir.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
                            if (setDay != 0 && setDay != today)
                            {
                                return false;
                            }

                            if (condition.TimeParameter.Hours != 0 && condition.TimeParameter.Hours != SEnvir.Now.Hour)
                            {
                                return false;
                            }

                            if (condition.TimeParameter.Minutes != 0 && condition.TimeParameter.Minutes != SEnvir.Now.Minute)
                            {
                                return false;
                            }

                            if (condition.TimeParameter.Seconds != 0 && condition.TimeParameter.Seconds != SEnvir.Now.Second)
                            {
                                return false;
                            }
                        }
                        break;
                }
            }

            return true;
        }

        public List<TriggerInfo> GetTriggers(TriggerConditionType type)
        {
            return SEnvir.TriggerInfoList.Binding
                .Where(x => x.TriggerConditions.Any(y =>
                    y.ConditionType == type && TriggerConditionSatisfied(y.Trigger))).ToList();
        }

        public List<TriggerCondition> GetTriggerConditions(TriggerConditionType type)
        {
            return SEnvir.TriggerInfoList.Binding.SelectMany(
                x => x.TriggerConditions).Where(
                y => y.ConditionType == type && TriggerConditionSatisfied(y.Trigger)).ToList();
        }

        public bool EvaluateTrigger(TriggerInfo trigger, bool check = false)
        {
            if (check)
            {
                if (!TriggerConditionSatisfied(trigger)) return false;
            }
            return SEnvir.Random.Next(100) < trigger.Probability;
        }

        public void ApplyTriggerEffect(TriggerEffect effect)
        {
            //todo 未完待续
            switch (effect.EffectType)
            {
                case TriggerEffectType.GainItem:
                    if (effect.ItemParameter == null) return;

                    ItemCheck check = new ItemCheck(effect.ItemParameter, 1, UserItemFlags.None, TimeSpan.Zero);
                    if (!CanGainItems(false, check)) return;
                    GainItem(SEnvir.CreateDropItem(check));

                    break;
            }
        }
    }
}
