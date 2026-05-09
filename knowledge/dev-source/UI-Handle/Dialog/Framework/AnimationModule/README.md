# Animation Module - Hệ Thống Animation Hoàn Chỉnh

## Tổng Quan
Animation Module là một hệ thống animation mạnh mẽ và đầy đủ tính năng cho Unity, được xây dựng trên DOTween. Hệ thống cung cấp giao diện trực quan để config animations trong Editor, hỗ trợ đầy đủ Canvas và non-Canvas objects.

## Cấu Trúc
```
AnimationModule/
├── Core/                    # Core classes
│   ├── AnimationManager.cs      # Component chính
│   ├── AnimationConfig.cs       # Data class cho animation config
│   ├── AnimationFactory.cs      # Factory tạo animations
├── Types/                   # Enum definitions
│   ├── AnimationType.cs         # Các loại animation
│   ├── PathType.cs
│   ├── PlayMode.cs
│   └── AutoPlayTrigger.cs
├── ScriptableObjects/       # Preset system
│   └── AnimationPresetSO.cs    # ScriptableObject để lưu presets
└── Editor/                  # Editor tools
    ├── AnimationManagerEditor.cs      # Custom Inspector
    ├── AnimationTimelineWindow.cs     # Visual Timeline Editor
    ├── PresetLibraryWindow.cs         # Preset Library Browser
    └── AnimationGizmoDrawer.cs        # Scene Gizmos
```

## Tính Năng Chính

### 1. Animation Types Đầy Đủ
- **Scale**: Phóng to/thu nhỏ
- **Move/LocalMove/AnchoredMove**: Di chuyển (World/Local/UI)
- **Rotation/LocalRotation**: Xoay
- **Fade/FadeCanvasGroup**: Mờ dần
- **Color**: Đổi màu (SpriteRenderer, Image, Text, Material)
- **Path**: Di chuyển theo đường dẫn
- **Shake** (Position/Rotation/Scale): Rung lắc
- **Punch** (Position/Rotation/Scale): Đấm nảy
- **SizeDelta**: Thay đổi kích thước RectTransform
- **Active**: Bật/tắt GameObject

### 2. Play Modes
- **Sequential**: Animations chạy tuần tự (A → B → C)
- **Parallel**: Animations chạy song song (A + B + C)
- **Mixed**: Kết hợp sequential và parallel

### 3. Ease Types
- Hỗ trợ tất cả DOTween Ease types
- Custom AnimationCurve
- Presets: Linear, Quad, Cubic, Bounce, Elastic, Back, Flash, etc.

### 4. Editor Features
- **Custom Inspector**: Giao diện trực quan, color-coded theo animation type
- **Visual Timeline Editor**: Timeline window với zoom, snap to grid
- **Preview System**: Preview animations trong Edit mode (không cần Play)
- **Preset System**: Save/Load animation configs
- **Preset Library**: Browse, search, filter presets
- **Scene Gizmos**: Visualization cho Move/Path animations

## Hướng Dẫn Sử Dụng

### Quick Start

1. **Tạo Animation Manager**
   - Add component `AnimationManager` vào GameObject
   - Hoặc: Menu → Component → Henry → Animation → Animation Manager

2. **Add Animations**
   - Trong Inspector, click "Add Animation"
   - Kéo target GameObject vào trường "Target"
   - Chọn Animation Type
   - Config các parameters (duration, ease, values, etc.)

3. **Config Play Mode**
   - **Sequential**: Animations chạy tuần tự
   - **Parallel**: Tất cả chạy cùng lúc
   - **Mixed**: Tự config từng animation (tick "Is Parallel")

4. **Auto Play**
   - Set "Auto Play Trigger":
     - **None**: Play manual
     - **OnAwake**: Play khi Awake
     - **OnStart**: Play khi Start
     - **OnEnable**: Play khi Enable

### Preview trong Editor

#### Method 1: Inspector Preview
1. Trong Inspector của AnimationManager
2. Click "Preview" button
3. Sử dụng progress slider để scrub timeline
4. Click "Stop Preview" để dừng

#### Method 2: Timeline Editor
1. Click "Open Timeline Editor" trong Inspector
2. Hoặc: Menu → Window → Henry → Animation Timeline
3. Click "Play" để preview
4. Kéo timeline marker để scrub
5. Zoom in/out với slider

### SửỤng Presets

#### Lưu Preset
1. Config xong animations trong AnimationManager
2. Click "Save as Preset"
3. Chọn vị trí lưu file
4. Đặt tên và lưu

#### Load Preset
1. Click "Load Preset"
2. Chọn file preset (.asset)
3. Animations sẽ được apply vào Manager

#### Preset Library
1. Click "Preset Library" hoặc Menu → Window → Henry → Animation Preset Library
2. Browse tất cả presets trong project
3. Search và filter theo category
4. Click "Apply" để apply preset
5. Duplicate/Edit/Delete presets

### Config Chi Tiết Cho Từng Animation Type

#### Scale Animation
```
- From Vector: Scale ban đầu (vd: 0,0,0)
- To Vector: Scale cuối (vd: 1,1,1)
- Duration: Thời gian animation
- Ease Type: Loại ease (OutQuad, OutBounce, etc.)
```

#### Move Animation
```
- From Position: Vị trí bắt đầu
- To Position: Vị trí kết thúc
- Move Type:
  - Move: World position
  - LocalMove: Local position
  - AnchoredMove: UI anchored position
```

#### Path Animation
```
- Path Points: Mảng các điểm path
- Path Type:
  - Linear: Đường thẳng giữa các điểm
  - CatmullRom: Đường cong mượt
- Path Resolution: Độ mịn của path
```

#### Shake Animation
```
- Strength: Cường độ rung
- Vibrato: Số lần rung (10 = default)
- Randomness: Độ ngẫu nhiên (0-90)
```

#### Punch Animation
```
- To Vector: Hướng và cường độ punch
- Vibrato: Số lần bounce
- Elasticity: Độ đàn hồi
```

#### Color Animation
```
- From Color: Màu ban đầu
- To Color: Màu cuối
- Tự động detect: SpriteRenderer, Image, Text, Material
```

#### Fade Animation
```
- From Value: Alpha ban đầu (0-1)
- To Value: Alpha cuối (0-1)
- Fade: Cho SpriteRenderer, Image, Text
- FadeCanvasGroup: Cho CanvasGroup (fade cả children)
```

### Loop Settings
```
- Loop: Bật/tắt loop
- Loop Count: Số lần lặp (-1 = infinite)
- Loop Type:
  - Restart: Lặp lại từ đầu
  - Yoyo: Đảo chiều mỗi lần
  - Incremental: Tăng dần
```

### Advanced Settings
```
- Use Unscaled Time: Không bị ảnh hưởng bởi Time.timeScale
- Update Type: Normal, Late, Fixed
- Auto Kill: Tự động xóa khi xong
```

### Events
```
- On Start: Gọi khi animation bắt đầu
- On Update: Gọi mỗi frame
- On Complete: Gọi khi kết thúc
- On Kill: Gọi khi bị kill
```

## API - Code Usage

### Play/Control Animations
```csharp
// Get reference
AnimationManager manager = GetComponent<AnimationManager>();

// Play
manager.Play();

// Pause/Resume
manager.Pause();
manager.Resume();

// Stop (về đầu)
manager.Stop();

// Restart
manager.Restart();

// Reverse
manager.Reverse();

// Set progress (0-1)
manager.SetProgress(0.5f);

// Set time scale
manager.SetTimeScale(2f); // 2x speed
```

### Subscribe Events
```csharp
manager.OnPlaybackStart += () => Debug.Log("Started!");
manager.OnPlaybackComplete += () => Debug.Log("Completed!");
manager.OnProgressUpdate += (progress) => Debug.Log($"Progress: {progress}");
```

### Get Info
```csharp
bool isPlaying = manager.IsPlaying;
bool isPaused = manager.IsPaused;
float progress = manager.CurrentProgress;
float duration = manager.TotalDuration;
```

### Dynamic Add Animations
```csharp
AnimationConfig config = new AnimationConfig();
config.target = targetObject;
config.animationType = AnimationType.Scale;
config.fromVector = Vector3.zero;
config.toVector = Vector3.one;
config.duration = 1f;
config.easeType = Ease.OutBounce;

manager.animations.Add(config);
manager.Play();
```

## Tips & Best Practices

### Performance
- Tránh tạo quá nhiều animations cùng lúc
- Sử dụng Auto Kill = true để tự động dọn dẹp
- Sử dụng Loop count hợp lý thay vì infinite khi có thể

### Canvas vs Non-Canvas
- Hệ thống tự detect RectTransform
- Với UI: Sử dụng AnchoredMove thay vì Move
- FadeCanvasGroup tốt hơn Fade cho UI groups

### Timeline
- Sử dụng Snap to Grid để align animations dễ dàng
- Mixed mode cho control tốt nhất
- Sequential mode đơn giản nhất

### Presets
- Tạo presets cho animations hay dùng
- Organize presets theo categories
- Share presets giữa các projects

### Gizmos
- Path animations hiển thị trong Scene view
- Move animations có arrows chỉ hướng
- Sử dụng để debug và visualize

## Keyboard Shortcuts (trong Timeline)
- **Mouse Wheel**: Scroll timeline
- **Click on Timeline**: Set playback time
- **Drag Animation Blocks**: (Coming soon - reorder animations)

## Troubleshooting

### Animation không chạy
- Kiểm tra target object có null không
- Kiểm tra duration > 0
- Kiểm tra Auto Play Trigger settings

### Preview không hoạt động
- Chỉ hoạt động trong Edit mode
- Kiểm tra target objects tồn tại trong scene
- Stop preview trước khi Play game

### Canvas animation không đúng
- Đảm bảo object có RectTransform
- Sử dụng AnchoredMove cho UI
- Kiểm tra Canvas settings

### Path không hiển thị
- Cần ít nhất 2 path points
- Kiểm tra trong Scene view (không phải Game view)
- Path points phải có giá trị hợp lý

## Staggered Animations (List Objects)

### Sử dụng StaggeredAnimationManager Component

Dùng khi muốn play animation cho nhiều objects với delay giữa chúng.

#### Setup trên Inspector

1. **Add Component**: `StaggeredAnimationManager`
2. **Add Objects**:
   - Kéo objects vào "Target Objects" list
   - Hoặc click "Auto-Fill Children" để tự động add children
3. **Set Animation Template**: Tạo AnimationConfig cho animation muốn play
4. **Config Stagger**:
   - Delay Between Objects: Thời gian delay giữa mỗi object
   - Play Order: Forward, Backward, Random, FromCenter, FromEdges
   - Play All Simultaneously: Play cùng lúc (overlap) hay sequential

#### Play Order Types

```
Forward:     0 → 1 → 2 → 3 → 4
Backward:    4 → 3 → 2 → 1 → 0
Random:      2 → 0 → 4 → 1 → 3
FromCenter:  2 → 1 → 3 → 0 → 4
FromEdges:   0 → 4 → 1 → 3 → 2
```

#### Code Example

```csharp
StaggeredAnimationManager staggerManager = GetComponent<StaggeredAnimationManager>();

// Play
staggerManager.Play();

// Dynamic add objects
staggerManager.AddObject(newObject);
staggerManager.SetObjects(objectArray);

// Events
staggerManager.OnObjectAnimationStart += (index, obj) =>
    Debug.Log($"Object {index} started!");
```

### Sử dụng Extension Methods (Quick & Easy)

#### Fade In/Out
```csharp
using HenryLe.Scripts.AnimationModule;

// Fade in list items
List<GameObject> items = GetListItems();
items.StaggeredFadeIn(duration: 0.5f, staggerDelay: 0.1f);

// Fade out
items.StaggeredFadeOut(0.3f, 0.05f);
```

#### Pop In/Out (Scale)
```csharp
// Pop in với bounce effect
items.StaggeredPopIn(duration: 0.6f, staggerDelay: 0.08f);

// Pop out
items.StaggeredPopOut(0.4f, 0.06f);
```

#### Custom Stagger Animations
```csharp
// Scale animation
items.DOStaggeredScale(
    to: Vector3.one,
    duration: 0.5f,
    staggerDelay: 0.1f,
    ease: Ease.OutBounce
);

// Fade animation
items.DOStaggeredFade(
    to: 1f,
    duration: 0.5f,
    staggerDelay: 0.1f,
    ease: Ease.OutQuad
);

// Move animation
Vector3[] positions = GetTargetPositions();
items.DOStaggeredMove(positions, 0.5f, 0.1f);

// UI Anchored position
Vector2[] uiPositions = GetUIPositions();
items.DOStaggeredAnchorPos(uiPositions, 0.5f, 0.1f);
```

#### From Center / From Edges
```csharp
// Animate from center ra ngoài
items.DOStaggeredFromCenter((obj, delay) => {
    obj.transform.DOScale(Vector3.one, 0.5f)
        .SetDelay(delay)
        .SetEase(Ease.OutBack);
}, staggerDelay: 0.1f);

// Animate from edges vào center
items.DOStaggeredFromEdges((obj, delay) => {
    obj.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f)
        .SetDelay(delay);
}, staggerDelay: 0.08f);
```

### Use Cases

#### Menu Items Fade In
```csharp
public class MenuController : MonoBehaviour
{
    public List<GameObject> menuItems;

    void Start()
    {
        // Hide all first
        menuItems.ForEach(item => {
            var cg = item.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0;
        });

        // Staggered fade in
        menuItems.StaggeredFadeIn(0.5f, 0.1f);
    }
}
```

#### List Items Pop In
```csharp
public class ItemList : MonoBehaviour
{
    public List<GameObject> listItems;

    public void ShowItems()
    {
        // All start at scale 0
        listItems.ForEach(item => item.transform.localScale = Vector3.zero);

        // Pop in with bounce
        listItems.DOStaggeredScale(Vector3.one, 0.6f, 0.08f, Ease.OutBack);
    }
}
```

#### Card Reveal Animation
```csharp
public class CardReveal : MonoBehaviour
{
    public List<GameObject> cards;

    public void RevealCards()
    {
        StaggeredAnimationManager stagger = gameObject.AddComponent<StaggeredAnimationManager>();
        stagger.SetObjects(cards);

        // Setup animation
        stagger.animationTemplate = new AnimationConfig
        {
            animationType = AnimationType.Scale,
            fromVector = Vector3.zero,
            toVector = Vector3.one,
            duration = 0.5f,
            easeType = Ease.OutBack
        };

        stagger.staggerDelay = 0.15f;
        stagger.staggerOrder = StaggerOrder.FromCenter;
        stagger.Play();
    }
}
```

#### Inventory Grid Fill
```csharp
public class InventoryGrid : MonoBehaviour
{
    public GameObject[] inventorySlots;

    void OnEnable()
    {
        // Auto-fill children
        StaggeredAnimationManager stagger = GetComponent<StaggeredAnimationManager>();

        // Auto-fill sẽ lấy tất cả children
        // Equivalent to manually adding all child GameObjects

        stagger.Play();
    }
}
```

### Performance Tips

- Stagger delay 0.05-0.15s thường là sweet spot
- Dùng `playAllSimultaneously = true` cho performance tốt hơn (all tweens cùng lúc)
- Với > 50 objects, consider batching hoặc object pooling
- Use RecyclableTween (đã tự động optimize)

---

## Support & Contact
Developed by HenryLe
Namespace: `HenryLe.Scripts.AnimationModule`
DOTween Version: Required
Unity Version: 2020.3+

---

**Enjoy animating! 🎬✨**
