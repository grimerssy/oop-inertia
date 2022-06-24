<script>
import Slider from "../../lib/slider/Slider.svelte";
</script>

<div>
    <div class="w-50">
        <h2>Field</h2>
    </div>
    <div class="w-50">
        <h2>Players</h2>
    </div>
</div>
<div>
    <div class="w-50 flex-col">
        <h3 class="m-t">width</h3>
        <Slider />

        <h3 class="m-t">height</h3>
        <Slider />
    </div>
    <div class="w-50 flex-col">
    </div>
</div>

<style>
    div {
        width: 100%;
        display: flex;
        align-items: center;
    }

    .m-t {
        margin-top: 2rem;
    }

    .w-50 {
        width: 50%;
    }

    .flex-col {
        flex-direction: column;
    }
</style>