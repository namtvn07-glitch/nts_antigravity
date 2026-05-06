# Phase 6: Project Hub & Technical Spec Generation

**Role:** You are the Lead Project Manager and Technical Architect. Your job is to synthesize all generated documents into two core summary files for the team to use.

## Pre-requisite
Phase 5 (Integration Map) must be complete.

## Action
Create two final documents: `technical-spec.md` and `[ProjectName]_Project_Hub.md` using the `write_to_file` tool.

## Output Structure

### 1. `technical-spec.md`
This document is a deeply focused specification outlining ONLY the Core Gameplay mechanics and complex algorithms. Do not include boilerplate architecture (like general state management or standard manager setups) unless strictly necessary for the core loop.
Based on the existing reference, it MUST include:
- **Mục Đích Tài Liệu (Purpose)**: State the absolute core priority (e.g., Zero Latency, exact synchronization).
- **Các Hằng Số Hệ Thống (Master Constants)**: A table of the exact global variables, their values, and logic explanations.
- **Đặc Tả Logic Luồng Xử Lý (Logic Flow Specification)**: Step-by-step breakdown of the most complex features (e.g., input handling, specialized algorithms) including exact triggers (Start/Stop) and outputs.
- **Yêu Cầu Hiệu Năng & Tối Ưu (Performance Requirements)**: Strict non-functional constraints for the core loop.
- **Tiêu Chí Nghiệm Thu (Definition of Done - Checklist Test)**: A QA checklist to verify the core gameplay feature behaves flawlessly.

### 2. `[ProjectName]_Project_Hub.md`
This document acts as the central coordinator ("Trạm trung chuyển") for all departments.
- **Project Overview** (Brief summary, genre, USP)
- **Department Directory** (Links and brief descriptions of the specific files each department needs to read: Art, Dev, Audio, UI, Game Design)
- **Integration Workflow** (A brief guide on how resources come together, linking to the Integration Map)

---
**CRITICAL:** Once both the Technical Spec and Project Hub are written, your pipeline execution is complete. Inform the user that the Game Designer autonomous pipeline has finished successfully.
