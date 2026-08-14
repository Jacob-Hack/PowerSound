# Changelog

Notable changes to PowerSound are documented here.

## Unreleased

### Fixed

- Fixed Start with Windows becoming out of sync when saved settings are enabled but the Windows startup entry is missing or broken.

### Changed

- Polished the README and download wording for casual users.
- Improved the GitHub release description template for future releases.

## v0.2.3

### Fixed

- Improved formatting of release notes shown by the built-in update checker.

### Changed

- Bumped the app version to 0.2.3 so existing installations can detect this update.

## v0.2.2

### Changed

- Bumped the app version to 0.2.2 so earlier installed builds can detect an available update.

## v0.2.1

### Added

- Added a custom PowerSound app, tray, and installer icon.
- Added About information.
- Added manual update checking.
- Added automatic update checks at startup.
- Added release notes to the update prompt.
- Added Reset All Settings to Defaults.
- Added MIT license and project creator credit.

### Changed

- Simplified the tray menu by removing the old AC Connected and AC Disconnected test commands.
- Improved uninstall and update cleanup when PowerSound is running.
- Cleaned up installer and publisher metadata.
- Renamed bundled sound assets to clearer names.
- Clarified the portable release artifact name and purpose.

## v0.2.0

### Added

- Added Battery Low, Battery Critical, Battery Emergency, and Fully Charged alerts.
- Added configurable thresholds for Low, Critical, and Emergency battery alerts.
- Added per-alert options to play a sound, show a Windows notification, both, or neither.
- Added bundled default sounds for AC power changes and battery alerts.
- Added support for custom `.wav` files.
- Added safe copying of selected custom sounds into `%APPDATA%\PowerSound\Sounds`.

### Changed

- Improved power-event handling so AC sounds only play when power status actually changes.
- Improved sound playback so longer WAV files do not block later power or battery events.
- Improved settings handling so Cancel does not save changes.
- Updated the default AC connected and disconnected sounds.

## v0.1.1

### Added

- Added an Inno Setup installer.
- Added automated GitHub Actions release builds.
- Added release download instructions.

## v0.1.0

### Added

- Added the initial PowerSound tray app.
- Added AC connected and AC disconnected sounds.
- Added built-in default sounds, custom sound selection, test buttons, saved settings, and Start with Windows support.
