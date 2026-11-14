using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FogboundFantasyArtBuilder
{
    private const string CharacterSource = "Assets/Art/KayKitFantasy/Characters";
    private const string DungeonSource = "Assets/Art/KayKitFantasy/Dungeon";
    private const string CharacterOutput = "Assets/Resources/Characters";
    private const string PropOutput = "Assets/Resources/DungeonProps";

    [MenuItem("Dungeon Rescue/Build Fantasy Art Pass")]
    public static void Build()
    {
        Directory.CreateDirectory(CharacterOutput);
        Directory.CreateDirectory(PropOutput);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var rogue = CreateMaterial("KayKit Rogue", $"{CharacterSource}/rogue_texture.png", .12f, .28f);
        var princess = CreateMaterial("KayKit Princess", $"{CharacterSource}/mage_texture.png", .08f, .3f);
        var skeleton = CreateMaterial("KayKit Skeleton", $"{CharacterSource}/skeleton_texture.png", .04f, .2f);
        var dungeon = CreateMaterial("KayKit Dungeon", $"{DungeonSource}/dungeon_texture.png", .08f, .25f);

        BuildCharacter("PlayerRogue", "Survivor", rogue, .76f, PlayerClips());
        BuildCharacter("PrincessMage", "Princess", princess, .68f, PrincessClips());
        BuildCharacter("SkeletonMinion", "Zombie", skeleton, .78f, EnemyClips(),
            new Attachment("Skeleton_Blade", "handslot.r"));
        BuildCharacter("SkeletonWarrior", "EliteZombie", skeleton, .68f, EnemyClips(),
            new Attachment("Skeleton_Axe", "handslot.r"),
            new Attachment("Skeleton_Shield_Large", "handslot.l"));
        BuildCharacter("SkeletonMage", "AbyssWarden", skeleton, .72f, BossClips(),
            new Attachment("Skeleton_Staff", "handslot.r"));

        BuildLongBlade(rogue);
        foreach (var source in Directory.GetFiles(DungeonSource, "*.fbx"))
            BuildProp(source.Replace('\\', '/'), dungeon);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("FOGBOUND_FANTASY_ART_BUILD_PASS characters=5 props=" + Directory.GetFiles(DungeonSource, "*.fbx").Length);
    }

    private static Dictionary<string, string> PlayerClips() => new()
    {
        ["Idle"] = "Idle",
        ["Idle_Gun"] = "2H_Ranged_Aiming",
        ["Walk"] = "Walking_A",
        ["Walk_Gun"] = "Walking_A",
        ["Run"] = "Running_A",
        ["Run_Gun"] = "Running_A",
        ["Slash"] = "2H_Melee_Attack_Slice",
        ["Idle_Attack"] = "2H_Ranged_Shoot",
        ["Death"] = "Death_A"
    };

    private static Dictionary<string, string> PrincessClips() => new()
    {
        ["Idle"] = "Idle",
        ["Cheer"] = "Cheer"
    };

    private static Dictionary<string, string> EnemyClips() => new()
    {
        ["Idle"] = "Idle_Combat",
        ["Walk"] = "Walking_D_Skeletons",
        ["Run"] = "Running_C",
        ["Idle_Attack"] = "1H_Melee_Attack_Chop",
        ["Death"] = "Death_C_Skeletons"
    };

    private static Dictionary<string, string> BossClips() => new()
    {
        ["Idle"] = "Spellcasting",
        ["Walk"] = "Walking_D_Skeletons",
        ["Run"] = "Running_C",
        ["Cleave"] = "2H_Melee_Attack_Slice",
        ["Charge"] = "Running_C",
        ["Cast"] = "Spellcast_Shoot",
        ["Summon"] = "Spellcast_Summon",
        ["Stun"] = "Hit_B",
        ["Death"] = "Death_C_Skeletons"
    };

    private static void BuildCharacter(string sourceName, string outputName, Material material, float scale,
        IReadOnlyDictionary<string, string> clips, params Attachment[] attachments)
    {
        var path = $"{CharacterSource}/{sourceName}.fbx";
        ConfigureCharacterImporter(path, clips);
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (source == null) throw new InvalidOperationException($"Missing character source: {path}");

        var root = new GameObject(outputName);
        var model = UnityEngine.Object.Instantiate(source, root.transform);
        model.name = "Model";
        model.transform.localScale = Vector3.one * scale;
        DisableEmbeddedEquipment(model.transform);
        ApplyMaterial(model, material);
        AddLegacyAnimation(model, path, clips.Keys);

        foreach (var attachment in attachments)
            AttachEquipment(model.transform, attachment, material);

        PrefabUtility.SaveAsPrefabAsset(root, $"{CharacterOutput}/{outputName}.prefab");
        UnityEngine.Object.DestroyImmediate(root);
    }

    private static void ConfigureCharacterImporter(string path, IReadOnlyDictionary<string, string> clips)
    {
        if (AssetImporter.GetAtPath(path) is not ModelImporter importer)
            throw new InvalidOperationException($"Missing model importer: {path}");
        importer.animationType = ModelImporterAnimationType.Legacy;
        importer.importAnimation = true;
        importer.importCameras = false;
        importer.importLights = false;
        importer.addCollider = false;
        importer.materialImportMode = ModelImporterMaterialImportMode.None;
        importer.clipAnimations = clips.Select(pair =>
        {
            var source = importer.defaultClipAnimations.FirstOrDefault(value => value.takeName == pair.Value);
            if (source == null) throw new InvalidOperationException($"{path} is missing clip {pair.Value}");
            source.name = pair.Key;
            source.wrapMode = pair.Key == "Death" ? WrapMode.ClampForever
                : pair.Key is "Slash" or "Idle_Attack" or "Cleave" or "Cast" or "Summon" or "Stun"
                    ? WrapMode.Once : WrapMode.Loop;
            source.loopTime = source.wrapMode == WrapMode.Loop;
            return source;
        }).ToArray();
        importer.SaveAndReimport();
    }

    private static void AddLegacyAnimation(GameObject model, string path, IEnumerable<string> clipNames)
    {
        var animation = model.GetComponent<Animation>() ?? model.AddComponent<Animation>();
        foreach (var clip in AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>()
                     .Where(value => clipNames.Contains(value.name)))
            animation.AddClip(clip, clip.name);
        animation.clip = animation["Idle"]?.clip;
        animation.playAutomatically = animation.clip != null;
        animation.cullingType = AnimationCullingType.AlwaysAnimate;
    }

    private static void DisableEmbeddedEquipment(Transform root)
    {
        var equipment = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Knife_Offhand", "1H_Crossbow", "2H_Crossbow", "Knife", "Throwable",
            "Spellbook", "Spellbook_open", "1H_Wand", "2H_Staff"
        };
        var unused = root.GetComponentsInChildren<Transform>(true)
            .Where(child => equipment.Contains(child.name)).ToArray();
        foreach (var child in unused) UnityEngine.Object.DestroyImmediate(child.gameObject);
    }

    private static void AttachEquipment(Transform model, Attachment attachment, Material material)
    {
        var socket = model.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(child => child.name.Equals(attachment.Socket, StringComparison.OrdinalIgnoreCase));
        var source = AssetDatabase.LoadAssetAtPath<GameObject>($"{CharacterSource}/{attachment.Source}.fbx");
        if (socket == null || source == null)
            throw new InvalidOperationException($"Cannot attach {attachment.Source} to {attachment.Socket}");
        var item = UnityEngine.Object.Instantiate(source, socket);
        item.name = attachment.Source;
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        item.transform.localScale = Vector3.one;
        ApplyMaterial(item, material);
        foreach (var collider in item.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
    }

    private static void BuildLongBlade(Material material)
    {
        const string sourcePath = CharacterSource + "/LongBladeSource.fbx";
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if (source == null) throw new InvalidOperationException("Missing long blade source.");
        var root = new GameObject("LongBlade");
        var model = UnityEngine.Object.Instantiate(source, root.transform);
        model.name = "Model";
        model.transform.localPosition = new Vector3(0f, 0f, .04f);
        model.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        model.transform.localScale = Vector3.one * .82f;
        ApplyMaterial(model, material);
        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Resources/Weapons/LongBlade.prefab");
        UnityEngine.Object.DestroyImmediate(root);
    }

    private static void BuildProp(string sourcePath, Material material)
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if (source == null) throw new InvalidOperationException($"Missing dungeon prop source: {sourcePath}");
        var name = Path.GetFileNameWithoutExtension(sourcePath);
        var root = new GameObject(name);
        var model = UnityEngine.Object.Instantiate(source, root.transform);
        model.name = "Model";
        ApplyMaterial(model, material);
        foreach (var collider in model.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
        PrefabUtility.SaveAsPrefabAsset(root, $"{PropOutput}/{name}.prefab");
        UnityEngine.Object.DestroyImmediate(root);
    }

    private static Material CreateMaterial(string name, string texturePath, float metallic, float smoothness)
    {
        var path = $"Assets/Resources/{name}.mat";
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        material.mainTexture = texture;
        material.color = Color.white;
        if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", Color.white);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void ApplyMaterial(GameObject target, Material material)
    {
        foreach (var renderer in target.GetComponentsInChildren<Renderer>(true))
            renderer.sharedMaterials = Enumerable.Repeat(material, renderer.sharedMaterials.Length).ToArray();
    }

    private readonly struct Attachment
    {
        public readonly string Source;
        public readonly string Socket;

        public Attachment(string source, string socket)
        {
            Source = source;
            Socket = socket;
        }
    }
}
