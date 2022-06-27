<script>
    import {onMount} from "svelte";

    let count = 5
    let leaderboard = []

    onMount(async () => {
        const response = await fetch(`http://localhost:8000/api/inertia/leaderboard/${count}`, {
            method: 'GET',
        })

        leaderboard = await response.json()
    })
</script>

<div class="col w-50">
    {#each leaderboard as keyValPair}
        <div class="row">
            <h2>{keyValPair.key}:</h2>
            <h2>{keyValPair.value}</h2>
        </div>
    {/each}
</div>

<style>
    h2 {
        margin: 0.3rem 0.7rem;
    }

    .w-50 {
        width: 50%;
    }

    .col {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .row {
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }
</style>