using System.Collections.Generic;
using Mx.Config;
using Mx.Res;
using UnityEngine;
using System.Linq;

namespace Mx.Sound
{
    /// <summary>声音模块的的父类</summary>
    public abstract class BaseSound : MonoBehaviour
    {
        #region 数据申明

        private SoundConfigDatabase soundConfig;
        private Dictionary<string, SoundItem> dicAllSounds = new Dictionary<string, SoundItem>();

        private bool mute = false;
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                MuteToggle();
            }
        }

        private float volume = 1;
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                SetVolume();
            }
        }

        #endregion

        #region 公开函数

        /// <summary>初始化</summary>
        public void Init(SoundConfigDatabase soundConfig)
        {
            this.soundConfig = soundConfig;
        }

        /// <summary>播放一个2D声音</summary>
        public void PlaySound2D(string soundName, bool isLoop = false, DelSoundCallback soundCallback = null)
        {
            PlaySound(soundName, null, isLoop, soundCallback);
        }

        /// <summary>播放一个3D声音</summary>
        public void PlaySound3D(string soundName, Transform target, bool isLoop = false, DelSoundCallback soundCallback = null)
        {
            PlaySound(soundName, target, isLoop, soundCallback);
        }

        /// <summary>设置循环播放</summary>
        public void SetLoop(string soundName, bool isLoop)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem != null) soundItem.Audio.loop = isLoop;
        }

        /// <summary>关闭声音播放</summary>
        public void CloseSounds(params string[] soundNames)
        {
            for (int i = 0; i < soundNames.Length; i++)
            {
                ChangeState(soundNames[i], SoundState.Stop);
            }
        }

        /// <summary>关闭所有声音播放</summary>
        public void CloseAllSounds()
        {
            string[] soundNames = dicAllSounds.Keys.ToArray<string>();
            CloseSounds(soundNames);
        }

        /// <summary>继续播放声音（从暂停状态进度继续播放）</summary>
        public void FurtherSoundPlayback(params string[] soundNames)
        {
            for (int i = 0; i < soundNames.Length; i++)
            {
                string soundName = soundNames[i];
                SoundItem soundItem = GetSoundItemByName(soundName);
                if (soundItem != null) soundItem.FurtherSoundPlayback();
            }
        }

        /// <summary>暂停声音播放</summary>
        public void PauseSounds(params string[] soundNames)
        {
            for (int i = 0; i < soundNames.Length; i++)
            {
                ChangeState(soundNames[i], SoundState.Pause);
            }
        }

        /// <summary>暂停所有声音播放</summary>
        public void PauseAllSounds()
        {
            string[] soundNames = dicAllSounds.Keys.ToArray<string>();
            PauseSounds(soundNames);
        }

        /// <summary>重新播放声音（进度从0开始）</summary>
        public void ReplaySounds(params string[] soundNames)
        {
            for (int i = 0; i < soundNames.Length; i++)
            {
                string soundName = soundNames[i];
                SoundItem soundItem = GetSoundItemByName(soundName);
                if (soundItem != null) soundItem.Replay();
            }
        }

        /// <summary>重新播放所有声音（进度从0开始）</summary>
        public void ReplayAllSounds()
        {
            string[] soundNames = dicAllSounds.Keys.ToArray<string>();
            ReplaySounds(soundNames);
        }

        /// <summary>移除声音及相关声音组件</summary>
        public void RemoveSounds(params string[] soundNames)
        {
            for (int i = 0; i < soundNames.Length; i++)
            {
                string soundName = soundNames[i];
                SoundItem soundItem = GetSoundItemByName(soundName);

                if (dicAllSounds.ContainsKey(soundName)) dicAllSounds.Remove(soundName);
                if (soundItem != null) Destroy(soundItem.gameObject);
            }
        }

        /// <summary>移除所有声音及相关声音组件</summary>
        public void RemoveAllSounds()
        {
            string[] soundNames = dicAllSounds.Keys.ToArray<string>();
            RemoveSounds(soundNames);
        }

        /// <summary>获取SoundItem</summary>
        public SoundItem GetSoundItemByName(string soundName)
        {
            SoundItem soundItem;
            dicAllSounds.TryGetValue(soundName, out soundItem);
            return soundItem;
        }

        /// <summary>获取声音状态</summary>
        public SoundState GetSoundStateByName(string soundName)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem == null) return SoundState.Error;
            return soundItem.State;
        }

        /// <summary>获取声音播放进度（取值范围：0-1）</summary>
        public float GetProgressByName(string soundName)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem == null) return 0;
            return soundItem.GetPlayProgress();
        }

        /// <summary>获取声音播放时间长度（单位：秒）</summary>
        public float GetPlayTimeByName(string soundName)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem == null) return 0;
            return soundItem.PlayTime;
        }

        /// <summary>获取声音时间长度（单位：秒）</summary>
        public float GetTimeByName(string soundName)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem == null) return 0;
            return soundItem.Time;
        }

        /// <summary>设置播放进度（0-1）</summary>
        public void SetProgress(string soundName, float progress)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem != null) soundItem.ChangeProgress(progress);
        }

        #endregion

        #region 私有函数

        private void PlaySound(string soundName, Transform target, bool isLoop, DelSoundCallback soundCallback)
        {
            SoundConfigData audioData = soundConfig.GetDataByKey(soundName);
            if (audioData == null)
            {
                Debug.LogError(GetType() + "/PlaySound()/ play audio error! soundName:" + soundName);
                return;
            }

            if (dicAllSounds.ContainsKey(soundName))
            {
                SoundItem soundItem = GetSoundItemByName(soundName);
                if (soundItem != null && soundItem.State != SoundState.Loading && soundItem.State != SoundState.Error)
                {
                    soundItem.SoundName = soundName;
                    soundItem.OnSoundCallback = soundCallback;
                    soundItem.target = target;
                    ChangeState(soundName, SoundState.Play);
                }
            }
            else
            {
                GameObject item = new GameObject(soundName);
                AudioSource audioSource = item.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = isLoop;
                SoundItem soundItem = item.AddComponent<SoundItem>();
                soundItem.SoundName = soundName;
                soundItem.OnSoundCallback = soundCallback;
                soundItem.target = target;
                soundItem.Audio = audioSource;
                dicAllSounds.Add(soundName, soundItem);
                ChangeState(soundName, SoundState.Loading);
            }
        }

        private void MuteToggle()
        {
            foreach (string soundName in dicAllSounds.Keys)
            {
                SoundItem soundItem = dicAllSounds[soundName];
                if (soundItem != null) soundItem.Audio.mute = Mute;
            }
        }

        private void SetVolume()
        {
            foreach (string soundName in dicAllSounds.Keys)
            {
                SoundItem soundItem = dicAllSounds[soundName];
                if (soundItem != null) soundItem.Audio.volume = Volume;
            }
        }

        private void ChangeState(string soundName, SoundState state)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            SoundConfigData audioData = soundConfig.GetDataByKey(soundName);
            if (audioData == null || soundItem == null) return;

            soundItem.State = state;

            switch (state)
            {
                case SoundState.Loading: LoadingState(audioData); break;
                case SoundState.Play: PlayState(soundName, soundItem); break;
                case SoundState.Pause: PauseState(soundItem); break;
                case SoundState.Stop: StopState(soundName, soundItem); break;
                case SoundState.Error: ErrorState(soundName); break;
            }
        }

        private void LoadingState(SoundConfigData audioData)
        {
            Load(audioData);
        }

        private void PlayState(string soundName, SoundItem soundItem)
        {
            soundItem.Play(Mute, volume);
            if (!dicAllSounds.ContainsKey(soundName)) dicAllSounds.Add(soundName, soundItem);
        }

        private void PauseState(SoundItem soundItem)
        {
            soundItem.Pause();
        }

        private void StopState(string soundName, SoundItem soundItem)
        {
            soundItem.Stop();
            if (dicAllSounds.ContainsKey(soundName)) dicAllSounds.Remove(soundName);
        }

        private void ErrorState(string soundName)
        {
            RemoveSounds(soundName);
        }

        private void Load(SoundConfigData audioData)
        {
            LoadType loadType = (LoadType)audioData.LandType;

            if (loadType == LoadType.Resources)
            {
                AudioClip audioClip = ResoucesMgr.Instance.Load<AudioClip>(audioData.ResourcesPath, false);
                if (audioClip != null) LoadFinish(audioData.Name, audioClip);
                else
                {
                    Debug.LogError(GetType() + "/Load()/ load audio error! soundName:" + audioData.Name);
                    ChangeState(audioData.Name, SoundState.Error);
                }
            }
            else if (loadType == LoadType.AssetBundle)
            {
                string sceneName = audioData.SceneName;
                string abName = audioData.AssetBundlePath;
                string assetName = audioData.AssetName;

                AssetBundleMgr.Instance.LoadAssetBunlde(sceneName, abName);
                AudioClip audioClip = AssetBundleMgr.Instance.LoadAsset(sceneName, abName, assetName) as AudioClip;
                LoadFinish(audioData.Name, audioClip);
            }
        }

        private void LoadFinish(string soundName, AudioClip audioClip)
        {
            SoundItem soundItem = GetSoundItemByName(soundName);
            if (soundItem == null) return;

            soundItem.Clip = audioClip;
            if (soundItem.State == SoundState.Loading) ChangeState(soundName, SoundState.Play);
        }

        #endregion
    }
}