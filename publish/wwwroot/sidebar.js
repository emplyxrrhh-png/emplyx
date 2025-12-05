document.addEventListener('DOMContentLoaded', function () {
  try {
    const sidebar = document.querySelector('.sidebar');
    if (!sidebar) return;

    // Sync width class with first label collapse state
    const firstLabel = sidebar.querySelector('.sidebar-collapse');
    if (firstLabel) {
      const collapse = bootstrap.Collapse.getOrCreateInstance(firstLabel);
      const update = () => {
        const shown = firstLabel.classList.contains('show');
        sidebar.classList.toggle('collapsed', !shown);
        sidebar.classList.toggle('expanded', shown);
      };
      firstLabel.addEventListener('shown.bs.collapse', update);
      firstLabel.addEventListener('hidden.bs.collapse', update);
      update();
    }
  } catch (e) {
    console.warn('sidebar.js init error', e);
  }
});

