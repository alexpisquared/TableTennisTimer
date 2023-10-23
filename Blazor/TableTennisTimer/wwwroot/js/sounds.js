window.PlayAudio = (elementName) => {
  var audio = document.getElementById(elementName);
  if (audio !== null && audio != null) {
    audio.play();
  }
}

window.PauseAudio = (elementName) => {
  var audio = document.getElementById(elementName);
  if (audio !== null && audio != null) {
    audio.pause();
  }
}