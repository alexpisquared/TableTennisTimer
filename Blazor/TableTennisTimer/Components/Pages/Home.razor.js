/* //todo: Jan 28: 
      https://progressier.com/pwa-capabilities/screen-wake-lock
      https://www.youtube.com/watch?v=HcVfL9o4LUw
 */
export function toggleWakeLock() {
  if (!navigator.wakeLock) {
    alert("Your device does not support the Wake Lock API. Try on an Android phone or on a device running iOS 16.4 or higher!");
  }
  else if (window.currentWakeLock && !window.currentWakeLock.released) {
    releaseScreen();
  }
  else {
    lockScreen();
  }
}

export function WakeLock() { lockScreen(); }
export function WakeUnlock() { unlockScreen(); }

async function lockScreen() {
  try {
    window.currentWakeLock = await navigator.wakeLock.request();
    document.getElementById('wake-lock-btn').innerHTML = "Release Wake Lock";
    alert('Wake Lock enabled');
  }
  catch (err) {
    alert(err);
  }
}

async function releaseScreen() {
  window.currentWakeLock.release();
  document.getElementById('wake-lock-btn').innerHTML = "Start Wake Lock";
  alert('Wake Lock released');
}