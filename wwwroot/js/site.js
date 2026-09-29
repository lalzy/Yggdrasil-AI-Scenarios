// Open Modals

document.addEventListener("click", (event) => {
    const button = event.target.closest("[data-modal]");
    if(!button) return;
    document.getElementById(button.dataset.modal).showModal();
});
