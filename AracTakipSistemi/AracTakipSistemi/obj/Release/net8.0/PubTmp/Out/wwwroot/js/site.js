// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Hamburger Menu Functionality - DEVRE DIŞI (Menü Artık Sabit)
document.addEventListener('DOMContentLoaded', function() {
    const hamburgerMenu = document.getElementById('hamburgerMenu');
    const sideMenu = document.getElementById('sideMenu');
    const menuOverlay = document.getElementById('menuOverlay');
    const closeMenu = document.getElementById('closeMenu');
    const submenuToggles = document.querySelectorAll('.submenu-toggle');
    
    // Menü her zaman açık olacak - açma/kapama işlevleri devre dışı
    
    // Submenu toggle functionality - Bu çalışmaya devam edecek
    submenuToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            const submenuId = this.getAttribute('data-submenu') + '-submenu';
            const submenu = document.getElementById(submenuId);
            const parentItem = this.parentElement;
            
            if (submenu) {
                // Close other submenus
                submenuToggles.forEach(otherToggle => {
                    if (otherToggle !== this) {
                        otherToggle.parentElement.classList.remove('active');
                        const otherSubmenuId = otherToggle.getAttribute('data-submenu') + '-submenu';
                        const otherSubmenu = document.getElementById(otherSubmenuId);
                        if (otherSubmenu) {
                            otherSubmenu.classList.remove('active');
                        }
                    }
                });
                
                // Toggle current submenu
                parentItem.classList.toggle('active');
                submenu.classList.toggle('active');
            }
        });
    });
    

});





// Loading animation for menu transitions
function showLoading() {
    const loader = document.createElement('div');
    loader.className = 'loading-spinner position-fixed';
    loader.style.top = '50%';
    loader.style.left = '50%';
    loader.style.transform = 'translate(-50%, -50%)';
    loader.style.zIndex = '9999';
    
    loader.innerHTML = `
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Yükleniyor...</span>
        </div>
    `;
    
    document.body.appendChild(loader);
    return loader;
}

function hideLoading(loader) {
    if (loader && loader.parentElement) {
        loader.remove();
    }
}

// Enhanced form validation feedback
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return false;
    
    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;
    
    inputs.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
            input.classList.add('is-valid');
        }
    });
    
    return isValid;
}

// Auto-hide alerts after 3 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert.parentElement) {
                alert.classList.remove('show');
                setTimeout(() => {
                    alert.remove();
                }, 150);
            }
        }, 3000);
    });
    
    // Bildirim sayısını güncelle
    updateBildirimSayisi();
    
    // Her 30 saniyede bir bildirim sayısını güncelle
    setInterval(updateBildirimSayisi, 30000);
});

// Bildirim sayısını güncelle
function updateBildirimSayisi() {
    $.get('/Bildirim/GetOkunmamisSayisi', function(response) {
        const badge = $('#bildirimBadge');
        if (response.sayi > 0) {
            badge.text(response.sayi).show();
        } else {
            badge.hide();
        }
    }).fail(function() {
        // Hata durumunda badge'i gizle
        $('#bildirimBadge').hide();
    });
}

// Smooth page transitions
document.addEventListener('DOMContentLoaded', function() {
    // Add fade-in effect to main content
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.style.opacity = '0';
        mainContent.style.transform = 'translateY(20px)';
        mainContent.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
        
        setTimeout(() => {
            mainContent.style.opacity = '1';
            mainContent.style.transform = 'translateY(0)';
        }, 100);
    }
});

// Enhanced accessibility features
document.addEventListener('DOMContentLoaded', function() {
    // Add focus management for hamburger menu
    const hamburgerMenu = document.getElementById('hamburgerMenu');
    const sideMenu = document.getElementById('sideMenu');
    const firstFocusableElement = sideMenu.querySelector('a, button, input, textarea, select');
    const lastFocusableElement = sideMenu.querySelector('a:last-child, button:last-child');
    
    hamburgerMenu.addEventListener('click', function() {
        setTimeout(() => {
            if (firstFocusableElement) {
                firstFocusableElement.focus();
            }
        }, 300);
    });
    
    // Trap focus within side menu when open
    document.addEventListener('keydown', function(e) {
        if (sideMenu.classList.contains('active')) {
            if (e.key === 'Tab') {
                if (e.shiftKey) {
                    if (document.activeElement === firstFocusableElement) {
                        e.preventDefault();
                        lastFocusableElement.focus();
                    }
                } else {
                    if (document.activeElement === lastFocusableElement) {
                        e.preventDefault();
                        firstFocusableElement.focus();
                    }
                }
            }
        }
    });
    
    // Menu search functionality
    const menuSearch = document.getElementById('menuSearch');
    if (menuSearch) {
        menuSearch.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const menuItems = document.querySelectorAll('.menu-item');
            
            menuItems.forEach(item => {
                const text = item.textContent.toLowerCase();
                if (text.includes(searchTerm)) {
                    item.style.display = 'block';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }
    
    // Context menu for advanced options
    document.addEventListener('contextmenu', function(e) {
        if (e.target.closest('.menu-link')) {
            e.preventDefault();
            // Show context menu for advanced options
            showContextMenu(e.pageX, e.pageY);
        }
    });
});

// Context menu functionality
function showContextMenu(x, y) {
    const contextMenu = document.createElement('div');
    contextMenu.className = 'context-menu';
    contextMenu.style.position = 'fixed';
    contextMenu.style.left = x + 'px';
    contextMenu.style.top = y + 'px';
    contextMenu.style.background = 'white';
    contextMenu.style.border = '1px solid #ccc';
    contextMenu.style.borderRadius = '4px';
    contextMenu.style.boxShadow = '0 2px 10px rgba(0,0,0,0.2)';
    contextMenu.style.zIndex = '10000';
    
    contextMenu.innerHTML = `
        <div class="context-menu-item" onclick="addToFavorites()">Favorilere Ekle</div>
        <div class="context-menu-item" onclick="copyLink()">Bağlantıyı Kopyala</div>
        <div class="context-menu-item" onclick="openInNewTab()">Yeni Sekmede Aç</div>
    `;
    
    document.body.appendChild(contextMenu);
    
    // Remove context menu when clicking elsewhere
    document.addEventListener('click', function() {
        if (contextMenu.parentElement) {
            contextMenu.remove();
        }
    }, { once: true });
}

// Context menu actions
function addToFavorites() {
    // Favorilere eklendi
}

function copyLink() {
    navigator.clipboard.writeText(window.location.href);
    // Bağlantı kopyalandı
}

function openInNewTab() {
    window.open(window.location.href, '_blank');
}

// Performance monitoring
document.addEventListener('DOMContentLoaded', function() {
    // Monitor page load time
    const loadTime = performance.timing.loadEventEnd - performance.timing.navigationStart;
    console.log('Page load time:', loadTime + 'ms');
    
    // Monitor menu open time
    const hamburgerMenu = document.getElementById('hamburgerMenu');
    if (hamburgerMenu) {
        hamburgerMenu.addEventListener('click', function() {
            const startTime = performance.now();
            setTimeout(() => {
                const endTime = performance.now();
                console.log('Menu open time:', (endTime - startTime) + 'ms');
            }, 300);
        });
    }
});
