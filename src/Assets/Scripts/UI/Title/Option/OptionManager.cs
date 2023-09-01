using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Navigation;
using Core.Utility.User;
using UI.Title.Option.Option1;
using UI.Title.Option.Option2;
using UI.Title.Option.Option3;
using UI.Title.Option.Option4;
using UnityEngine;

namespace UI.Title.Option
{
    public class OptionManager : AnimationBehaviour, IUIManager
    {
        [SerializeField] private Option1Manager option1;
        [SerializeField] private Option2Manager option2;
        [SerializeField] private Option3Manager option3;
        [SerializeField] private Option4Manager option4;

        [SerializeField] private SimpleUnderBarController bar;

        private Navigation<Nav> navigation;

        private UserPreference preference;

        protected override void Awake()
        {
            base.Awake();
            var initialManagers = new Dictionary<Nav, IUIManager>
            {
                { Nav.Option1, option1 },
                { Nav.Option2, option2 },
                { Nav.Option3, option3 },
                { Nav.Option4, option4 }
            };
            navigation = new Navigation<Nav>(initialManagers);

            SetNavigation();
        }


        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            navigation.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            navigation.SetScreen(Nav.Option1);
        }

        public void Select(Vector2 direction)
        {
        }

        public void Clicked()
        {
        }

        public void Finished(ContentTransform content, Action onFinished)
        {
            bar.AnimateOut(() =>
            {
                Animation(
                    content,
                    new AnimationListener
                    {
                        OnFinished = () =>
                        {
                            gameObject.SetActive(false);
                            navigation.SetActive(false);
                            onFinished?.Invoke();
                        }
                    }
                );
            });
        }

        public AnimationBehaviour GetContext()
        {
            return this;
        }

        public event Action OnBack;

        private void SetNavigation()
        {
            navigation.OnCancel += _ =>
            {
                switch (navigation.GetCurrentNav())
                {
                    case Nav.Option1:
                        OnBack?.Invoke();
                        break;
                    default:
                        navigation.SetScreen(Nav.Option1);
                        break;
                }
            };
            option1.OnClick += nav =>
            {
                switch (nav)
                {
                    case Option1Manager.Nav.Video:
                        NavigateToVideo();
                        break;
                    case Option1Manager.Nav.Audio:
                        NavigateToAudio();
                        break;
                    case Option1Manager.Nav.KeyConfig:
                        NavigateToKeyConfig();
                        break;
                }
            };
            option1.OnBack += () => { OnBack?.Invoke(); };

            option2.OnFullScreen += isOn =>
            {
                var data = preference.GetUserData();
                data.fullScreen.value = isOn;
                preference.SetUserData(data);
            };
            option2.OnBrightness += value =>
            {
                var data = preference.GetUserData();
                data.bright.value = (int)value;
                preference.SetUserData(data);
            };

            option2.OnBack += () =>
            {
                preference.Save();
                navigation.SetScreen(Nav.Option1);
            };

            option3.OnVolumeChanged += (nav, f) =>
            {
                var volume = (int)f;
                switch (nav)
                {
                    case Option3Manager.Nav.Master:
                        preference.SetMasterVolume(volume);
                        break;
                    case Option3Manager.Nav.Music:
                        preference.SetMusicVolume(volume);
                        break;
                    case Option3Manager.Nav.Effect:
                        preference.SetEffectVolume(volume);
                        break;
                }
            };
            option3.OnBack += () =>
            {
                preference.Save();
                navigation.SetScreen(Nav.Option1);
            };
            option4.OnBack += () =>
            {
                preference.Save();
                navigation.SetScreen(Nav.Option1);
            };
        }

        public void SetPreference(UserPreference manager)
        {
            preference = manager;
        }

        private void NavigateToVideo()
        {
            navigation.SetScreen(Nav.Option2);
            preference.Load();
            var user = preference.GetUserData();
            option2.SetValues(user.fullScreen.value, user.bright.value);
        }

        private void NavigateToAudio()
        {
            navigation.SetScreen(Nav.Option3);
            preference.Load();
            var user = preference.GetUserData();
            option3.SetValues(
                user.masterVolume.value,
                user.musicVolume.value,
                user.effectVolume.value
            );
        }

        private void NavigateToKeyConfig()
        {
            navigation.SetScreen(Nav.Option4);
        }

        private enum Nav
        {
            Option1,
            Option2,
            Option3,
            Option4
        }
    }
}