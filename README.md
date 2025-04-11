# Metavido VFX

![gif](https://github.com/user-attachments/assets/124a2b96-76d0-4e2a-8761-d2cc4ee1df72)
![gif](https://github.com/user-attachments/assets/078d9368-25ff-4fa8-99ed-0dbfadfc02b9)

**Metavido VFX** is a demonstration project that visualizes volumetric videos captured
with an iPhone Pro using its LiDAR sensor. It utilizes Unity’s VFX Graph and WebGPU to
create visually striking effects.

## Related Project

**Metavido** is an experimental project that captures volumetric videos with camera
tracking data using a burnt-in barcode extension. Please refer to the
[Metavido repository] for further details.

[Metavido repository]: https://github.com/keijiro/Metavido

## System Requirements

- Unity 6
- VFX Graph with URP or HDRP

## Web Browser Demo

A WebGPU build is available on [Unity Play].

[Unity Play]: https://play.unity.com/games/f4e0ea34-bd6d-4b2d-b24d-69ffa6e88795/metavido

To run it in your web browser, your environment must support WebGPU. It works on most
desktop browsers except Safari. It also runs on Chrome for Android.

For Safari on macOS or iOS, you must enable WebGPU manually using feature flags.
Follow the steps below to enable it.

###  Steps to enable WebGPU in Safari for iPhone

1. Open **Settings** on your iPhone.
2. Enable **Developer Mode** under *Privacy & Security > Developer Mode*.
3. Go to *Apps > Safari > Advanced > Feature Flags*, then enable **WebGPU**.
