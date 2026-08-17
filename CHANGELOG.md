# Changelog

Notable changes to PowerSound are documented here.

## Unreleased

## v1.0.0

Initial stable public release of PowerSound.

### Added

- Customizable sound when AC power is connected.
- Different customizable sound when AC power is disconnected.
- Built-in default sounds.
- Custom `.wav` sound support.
- Battery Low alert, enabled by default at 20%.
- Battery Critical alert, enabled by default at 10%.
- Battery Emergency alert, enabled by default at 5%.
- Optional Battery Fully Charged alert at 100%.
- Configurable Low, Critical, and Emergency battery thresholds.
- Per-alert options for sound, Windows notification, both, or neither.
- Custom sounds are copied into `%APPDATA%\PowerSound\Sounds` so they remain available if the original file moves or is deleted.
- Start with Windows support, enabled by default on fresh installs.
- Automatic update checks at startup.
- Manual Check for Updates.
- Update prompts with release notes and installer download/launch.
- Reset All Settings to Defaults.
- Windows installer and portable ZIP release options.
- Keyboard and screen-reader-friendly Windows controls.

### Changed

- Battery Low, Critical, and Emergency alerts trigger once per threshold crossing and do not repeat for every lower percent.
- Fully Charged triggers once per charge cycle.
- Update prompts show only the release's What's New notes instead of the full GitHub release page text.

### System Requirements

- Intended for 64-bit Windows 10 and Windows 11.
- Tested on Windows 11.
- Windows 10 should also work, but has not yet been tested.
