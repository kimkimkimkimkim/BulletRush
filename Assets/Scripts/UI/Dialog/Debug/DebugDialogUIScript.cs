using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace BulletRush.Debug
{
    public class DebugDialogUIScript : DialogBase
    {
        [SerializeField] protected Button _closeButton;
        [SerializeField] protected Button _restartButton;
        [SerializeField] protected Button _simulationButton;

        public override void Init(Dictionary<string, object> param)
        {
            var onClickClose = (Action)param["onClickClose"];

            _closeButton.OnClickIntentAsObservable(ButtonExtensions.ButtonClickIntent.OnlyOneTap)
                .SelectMany(_ => UIManager.Instance.CloseDialogObservable(gameObject))
                .Do(_ =>
                {
                    if (onClickClose != null)
                    {
                        onClickClose();
                        onClickClose = null;
                    }
                })
                .Subscribe();

            _restartButton.OnClickIntentAsObservable()
                .SelectMany(_ => CommonDialogFactory.Create(new CommonDialogRequest()
                {
                    title = "Confirm",
                    content = "プレイヤーデータを全て削除します。\nよろしいですか？"
                }))
                .Where(res => res.responseType == DialogUtil.DialogResponseType.Yes)
                .Do(_ =>
                {
                    SaveData.Clear();
                    SaveData.Save();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                })
                .Subscribe();

            _simulationButton.OnClickIntentAsObservable()
                .Do(_ => ExecuteSimulation())
                .Subscribe();

        }

        private void ExecuteSimulation()
        {
            const float intervalTime = 0.5f;
            var dps = 1.2f;
            var enemyList = new List<EnemyInfo>()
        {
            new EnemyInfo(14,Size.Medium,Type.Normal,0)
        };
            var spawnEnemyList = new List<EnemyInfo>(); // 現在出現中の敵リスト


        }
    }

    public class EnemyInfo
    {
        float hp;
        float currentHp;
        Size size;
        Type type;
        float spawnTime;

        public EnemyInfo(float hp, Size size, Type type, float spawnTime)
        {
            this.hp = hp;
            this.size = size;
            this.type = type;
            this.spawnTime = spawnTime;
        }
    }

    public enum Size
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }

    public enum Type
    {
        Normal,
        Homing,
        Block,
        Fast,
        Slow,
        Random,
    }
}
