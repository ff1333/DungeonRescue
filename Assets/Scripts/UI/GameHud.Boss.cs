using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FogboundMaze
{
    public sealed partial class GameHud
    {
        private GameObject bossHud;
        private Image bossHealthFill;
        private Text bossHealthText;
        private Text bossPhaseText;
        private Text bossMessage;
        private Coroutine bossMessageRoutine;

        private void BuildBossHud()
        {
            bossHud = PanelObject("Boss HUD", transform, new Color(.035f,.025f,.045f,.94f));
            SetRect(bossHud.GetComponent<RectTransform>(), new Vector2(.26f,.79f), new Vector2(.74f,.875f));
            AddFrame(bossHud);
            bossPhaseText = Label("Boss Name", bossHud.transform, "ABYSS WARDEN", 25, TextAnchor.MiddleLeft,
                new Color(1f,.65f,.82f));
            SetRect(bossPhaseText.rectTransform,new Vector2(.03f,.48f),new Vector2(.62f,.96f));
            bossHealthText = Label("Boss Health",bossHud.transform,"1800 / 1800",21,TextAnchor.MiddleRight);
            SetRect(bossHealthText.rectTransform,new Vector2(.62f,.48f),new Vector2(.97f,.96f));
            var track = PanelObject("Boss Health Track",bossHud.transform,new Color(.16f,.12f,.18f));
            SetRect(track.GetComponent<RectTransform>(),new Vector2(.03f,.14f),new Vector2(.97f,.40f));
            bossHealthFill = PanelObject("Boss Health Fill",track.transform,new Color(.82f,.12f,.38f)).GetComponent<Image>();
            SetRect(bossHealthFill.rectTransform,Vector2.zero,Vector2.one);

            bossMessage = Label("Boss Message",transform,string.Empty,34,TextAnchor.MiddleCenter,
                new Color(1f,.84f,.58f));
            SetRect(bossMessage.rectTransform,new Vector2(.18f,.68f),new Vector2(.82f,.80f));
            bossMessage.gameObject.SetActive(false);
            bossHud.SetActive(false);
        }

        public void ShowBossIntro(string bossName, string objective)
        {
            bossHud.SetActive(true);
            bossPhaseText.text = $"{bossName}  ·  PHASE ONE";
            ShowBossMessage($"FINAL BATTLE: {objective}", 2.8f);
        }

        public void RefreshBoss(float current, float maximum, BossPhase phase)
        {
            bossHud.SetActive(phase != BossPhase.Defeated);
            var fraction = maximum > 0f ? Mathf.Clamp01(current / maximum) : 0f;
            bossHealthFill.rectTransform.anchorMax = new Vector2(fraction, 1f);
            bossHealthFill.rectTransform.offsetMin = Vector2.zero;
            bossHealthFill.rectTransform.offsetMax = Vector2.zero;
            bossHealthText.text = $"{current:0} / {maximum:0}";
        }

        public void ShowBossPhase(BossPhase phase)
        {
            if (phase == BossPhase.Defeated)
            {
                bossHud.SetActive(false);
                return;
            }
            var label = phase switch
            {
                BossPhase.PhaseTwo => "PHASE TWO · SUMMON AND SHOCKWAVE",
                BossPhase.PhaseThree => "PHASE THREE · ABYSS CORRUPTION",
                BossPhase.Stunned => "WEAK POINT EXPOSED",
                _ => "PHASE ONE"
            };
            bossPhaseText.text = $"ABYSS WARDEN  ·  {label}";
            if (phase is BossPhase.PhaseTwo or BossPhase.PhaseThree) ShowBossMessage(label, 1.8f);
        }

        public void ShowRescuePrompt(string message)
        {
            bossHud.SetActive(false);
            ShowBossMessage(message, 8f);
        }

        private void ShowBossMessage(string message, float duration)
        {
            if (bossMessageRoutine != null) StopCoroutine(bossMessageRoutine);
            bossMessage.text = message;
            bossMessage.gameObject.SetActive(true);
            bossMessageRoutine = StartCoroutine(HideBossMessage(duration));
        }

        private IEnumerator HideBossMessage(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            bossMessage.gameObject.SetActive(false);
            bossMessageRoutine = null;
        }
    }
}
