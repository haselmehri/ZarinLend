export class BaseElement {
    constructor(label) {
        this.label = label;
        this.uniqueId = this.generateUniqueId();
    }

    generateUniqueId() {
        return 'xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    init() {
        let baseDiv = document.createElement('div');
        baseDiv.setAttribute('class', 'form-group');
        baseDiv.setAttribute('id', this.uniqueId);
        baseDiv.setAttribute('component-type', this.type);

        let label = document.createElement('label');
        label.setAttribute('id', `lbl_${this.uniqueId}`);
        label.innerText = this.label;

        baseDiv.appendChild(label);
        return baseDiv;
    }
}