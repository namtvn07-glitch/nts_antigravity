## Core Pattern

### 1. mandatory Profiler Usage
Performance checking is NOT an afterthought. It is a continuous development requirement.
- **Frame Debugger**: Use this to analyze draw calls, check batching efficiency, and prevent overdraw.
- **Memory Profiler**: Use this to hunt memory leaks, bloated texture loads, and string allocations. Address massive GC Spikes directly here.

### 2. Target Performance Limits
Do not blindly write logic assuming it runs at 144fps. Assume the worst-case scenario.
- **Mobile**: Strict < 16ms (60fps) or < 33ms (30fps) thresholds. Limit poly count drastically and optimize physics matrices.
- **Avoid Heavy Physics Hooks**: `OnCollisionStay` and deep Raycast loops are massive CPU bottlenecks.
- Never let UI rebuilds (Dirty Canvas) happen consistently every frame.

### 3. Automated QA & Unity Test Framework
- Implement **PlayMode** and **EditMode** tests for critical core logic and math operations.
- Ensure automated regression tests exist so future code changes don’t silently break underlying performance.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "The game runs smoothly in the Unity Editor at 120fps, so we are good." | Editor performance is a lie. PC CPU brute forces the unoptimized garbage. Test on target devices and rely strictly on the Profiler. |
| "It's just a 2D game, I don't need to profile it." | 2D lighting, massive transparent overlaps (overdraw), and Physics2D can destroy Mobile framerates instantly. Profile everything. |
| "I’ll optimize it later right before release." | Architectural rewrites right before release cause critical bugs. Optimize from Day 1 using DevArchitecture rules. |

**Any Red Flags appearing: REJECT CODE, MANDATE IMMEDIATE PROFILING AND REFACTORING OF BOTTLENECKS.**
