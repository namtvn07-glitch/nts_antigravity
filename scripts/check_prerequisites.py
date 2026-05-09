"""
check_prerequisites.py — Fail-fast prerequisite checker for game-* skills.

Usage:
    python .agents/scripts/check_prerequisites.py <skill_name> <project_path>

Exit codes:
    0 = PASS (skill can proceed)
    1 = FAIL (required artifacts missing)
    2 = UNKNOWN skill (no prereqs defined, treated as PASS)
"""
import sys
import os
import glob

PREREQUISITES = {
    "game-art-orchestrator": {
        "description": "Requires compiled style DNA from game-art-compiler.",
        "requires_any": [
            "**/Generation_DNA.md",
            "**/style_index.json",
        ],
        "message": (
            "No style DNA found. Run `game-art-compiler` first to compile "
            "a style directory into Generation_DNA.md and style_index.json."
        ),
    },
    "game-audio-prompter": {
        "description": "Requires GDD and audio asset list from game-designer.",
        "requires_all": [
            "**/*_GDD.md",
            "**/*_Audio_Assets.json",
        ],
        "message": (
            "Missing GDD or Audio_Assets.json. "
            "Run `game-designer` first to generate project documents."
        ),
    },
    "game-aso-orchestrator": {
        "description": "Requires a project GDD from game-designer.",
        "requires_any": ["**/*_GDD.md", "**/*Master_GDD.md"],
        "message": (
            "No GDD found. Run `game-designer` first to generate the Master GDD."
        ),
    },
    "game-playable-orchestrator": {
        "description": "Requires a completed game project with GDD.",
        "requires_any": ["**/*_GDD.md", "**/*Master_GDD.md"],
        "message": (
            "No GDD found. A completed game project with GDD is required. "
            "Run `game-designer` first."
        ),
    },
    "game-dev-unity": {
        "description": "Requires a project GDD or technical spec.",
        "requires_any": ["**/*_GDD.md", "**/*technical-spec.md"],
        "message": (
            "No GDD or technical spec found. "
            "Run `game-designer` to generate project architecture documents."
        ),
    },
}


def glob_find(project_path, pattern):
    """Search for files matching glob pattern under project_path."""
    return glob.glob(os.path.join(project_path, pattern), recursive=True)


def check_prerequisites(skill_name, project_path):
    config = PREREQUISITES.get(skill_name)

    if config is None:
        print(f"[PASS] No prerequisites defined for '{skill_name}'. Skill can proceed.")
        return True

    print(f"[CHECK] {skill_name}: {config['description']}")

    # Check requires_any (at least one must exist)
    if "requires_any" in config:
        found = False
        for pattern in config["requires_any"]:
            matches = glob_find(project_path, pattern)
            if matches:
                print(f"  [FOUND] {matches[0]}")
                found = True
                break
        if not found:
            print(f"  [FAIL] {config['message']}")
            print(f"  Searched in: {project_path}")
            return False

    # Check requires_all (every pattern must have at least one match)
    if "requires_all" in config:
        for pattern in config["requires_all"]:
            matches = glob_find(project_path, pattern)
            if not matches:
                print(f"  [FAIL] Missing: {pattern}")
                print(f"  {config['message']}")
                return False
            else:
                print(f"  [FOUND] {matches[0]}")

    print(f"[PASS] All prerequisites satisfied for '{skill_name}'.")
    return True


if __name__ == "__main__":
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(2)

    skill = sys.argv[1]
    path = os.path.abspath(sys.argv[2])

    if not os.path.isdir(path):
        print(f"[ERROR] Project path not found: {path}")
        sys.exit(1)

    success = check_prerequisites(skill, path)
    sys.exit(0 if success else 1)
