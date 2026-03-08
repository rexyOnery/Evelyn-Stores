// EvelynChart namespace to render charts from Blazor
window.EvelynChart = window.EvelynChart || {};

window.EvelynChart.renderTopCategory = function (labels, data, colors) {
    var canvas = document.getElementById('top-category');
    if (!canvas) return;

    var ctx = canvas.getContext('2d');

    // destroy previous instance if exists
    if (window._evelynTopCategoryChart) {
        try { window._evelynTopCategoryChart.destroy(); } catch (e) { /* ignore */ }
        window._evelynTopCategoryChart = null;
    }

    var useLabels = (labels && labels.length) ? labels : ['No data'];
    var useData = (data && data.length) ? data : [1];
    var useColors = (colors && colors.length) ? colors : ['#092C4C', '#E04F16', '#FE9F43'];

    window._evelynTopCategoryChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: useLabels,
            datasets: [{
                label: 'Categories',
                data: useData,
                backgroundColor: useColors,
                borderWidth: 5,
                borderRadius: 10,
                borderColor: '#fff',
                hoverBorderWidth: 0,
                cutout: '50%'
            }]
        },
        options: {
            layout: {
                padding: {
                    top: 0,
                    bottom: 0
                }
            },
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            }
        }
    });
};

// Backwards compatibility: render default static chart if no dynamic call
document.addEventListener('DOMContentLoaded', function () {
    var canvas = document.getElementById('top-category');
    if (canvas && !window._evelynTopCategoryChart) {
        window.EvelynChart.renderTopCategory(['Lifestyles','Sports','Electronics'], [16,24,50], ['#092C4C', '#E04F16', '#FE9F43']);
    }
});
