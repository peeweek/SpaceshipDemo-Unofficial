using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;
using UnityEngine.VFX;
using System.Collections.Generic;

public class VFXProfilerCounters : MonoBehaviour
{
    ProfilerCounter<int> vfxCounter;

    ProfilerCounter<int> vfxParticleCounter;

    ProfilerCounter<int> vfxActiveCounter;
    ProfilerCounter<int> vfxCulledCounter;
    ProfilerCounter<int> vfxSleepingCounter;

    ProfilerCounter<int> vfxSystemCounter;
    ProfilerCounter<int> vfxAliveSystemCounter;

    ProfilerCounter<int> vfxSpawnSystemCounter;
    ProfilerCounter<int> vfxSpawnSystemSpawningCounter;

    static ProfilerCategory category = new ProfilerCategory("VFX");

    ProfilerCounter<float> vfxUpdateTimeCounter;
    ProfilerCounter<int> vfxInitCounter;
    ProfilerCounter<int> vfxBatchInitCounter;
    ProfilerCounter<int> vfxUpdateCounter;
    ProfilerCounter<int> vfxBatchUpdateCounter;

    List<VFXProfilerRecordFowrarder> vfxProfilerForwarders = new List<VFXProfilerRecordFowrarder>();

    public class VFXProfilerRecordFowrarder
    {
        static ProfilerCategory lowLevelcategory = new ProfilerCategory("VFX (Low Level)");
        bool ignoreRenderThreadDuplicates = false;
        ProfilerRecorder recorder;

        ProfilerCounterValue<int> sampleCount;
        ProfilerCounterValue<long> sampleTime;

        public VFXProfilerRecordFowrarder(string sampleName, bool ignoreRenderThreadDuplicates = false)
        {
            this.ignoreRenderThreadDuplicates = ignoreRenderThreadDuplicates;
            this.sampleCount = new ProfilerCounterValue<int>(lowLevelcategory, sampleName + " (Count)", ProfilerMarkerDataUnit.Count, ProfilerCounterOptions.FlushOnEndOfFrame | ProfilerCounterOptions.ResetToZeroOnFlush );
            this.sampleTime = new ProfilerCounterValue<long>(lowLevelcategory, sampleName + " (Time)", ProfilerMarkerDataUnit.TimeNanoseconds, ProfilerCounterOptions.None);
            this.recorder = ProfilerRecorder.StartNew(ProfilerCategory.Particles, sampleName, 20, ProfilerRecorderOptions.Default | ProfilerRecorderOptions.CollectOnlyOnCurrentThread);
        }

        public void Update()
        {
            int idx = this.recorder.Count - 1;
            if(idx >= 0 && idx < this.recorder.Capacity)
            {
                var sample = this.recorder.GetSample(idx);
                this.sampleCount.Value = (int)(sample.Count * (this.ignoreRenderThreadDuplicates ? 0.5f : 1f));
                this.sampleTime.Value = sample.Value;
            }
        }
    }


    void Start()
    {
        vfxCounter = new ProfilerCounter<int>(category, "VFX Count (Total)", ProfilerMarkerDataUnit.Count);
        vfxParticleCounter = new ProfilerCounter<int>(category, "VFX Particle Count (Total)", ProfilerMarkerDataUnit.Count);
        vfxActiveCounter = new ProfilerCounter<int>(category, "VFX Count (Active)", ProfilerMarkerDataUnit.Count);
        vfxCulledCounter = new ProfilerCounter<int>(category, "VFX Count (Culled)", ProfilerMarkerDataUnit.Count);
        vfxSleepingCounter = new ProfilerCounter<int>(category, "VFX Count (Sleeping)", ProfilerMarkerDataUnit.Count);
        vfxSystemCounter = new ProfilerCounter<int>(category, "VFX Particle Systems (Total)", ProfilerMarkerDataUnit.Count);
        vfxAliveSystemCounter = new ProfilerCounter<int>(category, "VFX Particle Systems (Alive)", ProfilerMarkerDataUnit.Count);
        vfxSpawnSystemCounter = new ProfilerCounter<int>(category, "VFX Spawn Systems (Total)", ProfilerMarkerDataUnit.Count);
        vfxSpawnSystemSpawningCounter = new ProfilerCounter<int>(category, "VFX Spawn Systems (Spawning)", ProfilerMarkerDataUnit.Count);

        AddProfilerForwarder("VFX.ParticleSystem.Init", true);
        AddProfilerForwarder("VFX.ParticleSystem.BatchInit", true);
        AddProfilerForwarder("VFX.ParticleSystem.Update", true);
        AddProfilerForwarder("VFX.ParticleSystem.BatchUpdate", true);
    }

    void AddProfilerForwarder(string statName, bool duplicatesOnRenderThread)
    {
        var pr = new ProfilerRecorder(ProfilerCategory.Render, statName, options: ProfilerRecorderOptions.SumAllSamplesInFrame);
        vfxProfilerForwarders.Add(new VFXProfilerRecordFowrarder(statName, duplicatesOnRenderThread));
    }

    VisualEffect[] components = null;
    Dictionary<VisualEffectAsset, List<string>> systemNames = new Dictionary<VisualEffectAsset, List<string>>();
    Dictionary<VisualEffectAsset, List<string>> spawnSystemNames = new Dictionary<VisualEffectAsset, List<string>>();



    private void Update()
    {
        components = FindObjectsByType<VisualEffect>(FindObjectsSortMode.None);
        int length = components.Length;
        vfxCounter.Sample(length);
        int active = 0;
        int culled = 0;
        int sleeping = 0;

        int systemsTotal = 0;
        int systemsAlive = 0;

        int spawnSystemsTotal = 0;
        int spawnSystemsSpawning = 0;
        int particleCount = 0;

        for (int i = 0; i < length; i++)
        {
            var vfx = components[i];
            active += vfx.isActiveAndEnabled ? 1 : 0;
            culled += vfx.culled ? 1 : 0;
            bool awake = vfx.HasAnySystemAwake();
            sleeping += awake ? 0 : 1;
            particleCount += vfx.aliveParticleCount;

            // Caching System names by asset, to reduce GC garbage

            if(!systemNames.ContainsKey(vfx.visualEffectAsset))
            {
                systemNames.Add(vfx.visualEffectAsset, new List<string>());
                spawnSystemNames.Add(vfx.visualEffectAsset, new List<string>());

                vfx.GetSystemNames(systemNames[vfx.visualEffectAsset]);
                vfx.GetSpawnSystemNames(spawnSystemNames[vfx.visualEffectAsset]);
            }

            foreach (var system in systemNames[vfx.visualEffectAsset])
            {
                var info = vfx.GetParticleSystemInfo(system);
                systemsTotal++;
                if (!info.sleeping)
                    systemsAlive++;
            }

            foreach (var system in spawnSystemNames[vfx.visualEffectAsset])
            {
                var info = vfx.GetSpawnSystemInfo(system);
                spawnSystemsTotal++;
                if (info.spawnCount > 0)
                    spawnSystemsSpawning++;
            }

        }


        vfxParticleCounter.Sample(particleCount);

        vfxActiveCounter.Sample(active);
        vfxCulledCounter.Sample(culled);
        vfxSleepingCounter.Sample(sleeping);

        vfxSystemCounter.Sample(systemsTotal);
        vfxAliveSystemCounter.Sample(systemsAlive);

        vfxSpawnSystemCounter.Sample(spawnSystemsTotal);
        vfxSpawnSystemSpawningCounter.Sample(spawnSystemsSpawning);

        // Update all sample Forwarders
        foreach(var forwarder in vfxProfilerForwarders)
        {
            forwarder.Update();
        }

    }
}
