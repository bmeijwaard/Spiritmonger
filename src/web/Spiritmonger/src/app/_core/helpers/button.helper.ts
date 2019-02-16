export class ButtonHelper {
  static spinnerHtml = '<i class="fas fa-cog fa-spin"></i>';

  static toggleButtonByClass(buttonClass: string, buttonText: string = null): string {
      const button = <HTMLScriptElement>(document.getElementsByClassName(buttonClass)[0]);
      return this.toggleHelper(button, buttonText);
  }

  static toggleButtonById(buttonId: string, buttonText: string = null): string {
      const button = document.getElementById(buttonId);
      console.log(button);
      return this.toggleHelper(button, buttonText);
  }

  private static toggleHelper(button: HTMLElement, buttonText: string): string {
      let result;
      if (buttonText === null) {
          button.setAttribute('disabled', 'true');
          result = button.innerHTML;
          const width = button.getBoundingClientRect().width;
          button.style.width = width + 'px';
          button.innerHTML = this.spinnerHtml;
          return result;
      } else {
          button.removeAttribute('disabled');
          button.style.width = null;
          button.innerHTML = buttonText;
          return null;
      }
  }
}
