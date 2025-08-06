/**
 * Opens a URL in a popup window and returns a promise that resolves when the popup is closed.
 * @param url The URL to open in the popup.
 * @param windowName A name for the new window.
 * @returns A promise that resolves when the popup is closed by the user or by redirection.
 */
export const openPopup = (url: string, windowName: string): Promise<void> => {
  const width = 600;
  const height = 800;
  const left = window.screenX + (window.outerWidth - width) / 2;
  const top = window.screenY + (window.outerHeight - height) / 2;

  const popup = window.open(
    url,
    windowName,
    `width=${width},height=${height},left=${left},top=${top},toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes`
  );

  return new Promise((resolve) => {
    if (!popup) {
      
      alert("Popup was blocked. Please allow popups for this site to continue.");
      return resolve();
    }

    const timer = setInterval(() => {
      
      if (popup.closed) {
        clearInterval(timer);
        resolve();
      }
    }, 500); 
  });
};