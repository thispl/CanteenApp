// Sidebar Toggle Functionality
(function() {
    'use strict';
    
    // Check for saved preference
    const SIDEBAR_COLLAPSED_KEY = 'cms_sidebar_collapsed';
    
    function initSidebarToggle() {
        // Create toggle button
        const toggleBtn = document.createElement('button');
        toggleBtn.className = 'sidebar-toggle-btn';
        toggleBtn.id = 'sidebarToggleBtn';
        toggleBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
        toggleBtn.title = 'Toggle Sidebar';
        
        // Add click handler
        toggleBtn.addEventListener('click', function() {
            toggleSidebar();
        });
        
        // Add to body when DOM is ready
        if (document.body) {
            document.body.appendChild(toggleBtn);
        } else {
            document.addEventListener('DOMContentLoaded', function() {
                document.body.appendChild(toggleBtn);
            });
        }
        
        // Apply saved state
        const isCollapsed = localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === 'true';
        if (isCollapsed) {
            document.body.classList.add('lpx-sidebar-collapsed');
            updateToggleIcon(true);
        }
    }
    
    function toggleSidebar() {
        const body = document.body;
        const isCollapsed = body.classList.toggle('lpx-sidebar-collapsed');
        
        // Save preference
        localStorage.setItem(SIDEBAR_COLLAPSED_KEY, isCollapsed);
        
        // Update icon
        updateToggleIcon(isCollapsed);
        
        // Trigger resize event for any charts/tables
        window.dispatchEvent(new Event('resize'));
    }
    
    function updateToggleIcon(isCollapsed) {
        const btn = document.getElementById('sidebarToggleBtn');
        if (btn) {
            btn.innerHTML = isCollapsed 
                ? '<i class="fas fa-chevron-right"></i>' 
                : '<i class="fas fa-chevron-left"></i>';
        }
    }
    
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSidebarToggle);
    } else {
        initSidebarToggle();
    }
})();
